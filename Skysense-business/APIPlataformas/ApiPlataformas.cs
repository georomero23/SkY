using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Skysense_business.Comun;
using Skysense_models.APIS;
using Skysense_models.APIS.Huawei;
using Skysense_models.Otros;
using Skysense_persistencia.Entidades;
using System.Globalization;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;

namespace Skysense_business.APIPlataformas
{
    public abstract class ApiPlataforma
    {
        protected readonly string _apiUrlBase;
        protected readonly string _apiUser;
        protected readonly string _apiPwd;
        public ApiPlataforma(string apiUrlBase, string apiUser, string apiPwd) {
            this._apiUrlBase = apiUrlBase;
            this._apiUser = apiUser;
            this._apiPwd = apiPwd;
        }
        public abstract Task<decimal?[]?> mObtenGeneracionAnualInversor(int iAnio, string idInstalacionAPI);
        //public abstract Task<InversorData[]> mObtenInfoInversoresDiario(string idInstalacionAPI, DateTime fecha);
        public abstract Task<Dictionary<string, InversorInfo[]>> mObtenListaInversores(params string[] idInstalacionAPI);
        public abstract Task<InversorDataDiaria[]> mObtenGeneracionDiariaInversores(string idInstalacionAPI, DateTime fecha, params InversorInfo[] arrInversores);
        public abstract Task<InversorDataDiaria[]> mMonitoreaGeneracionTiempoReal(string idInstalacionAPI, params InversorInfo[] arrInversores);
    }

    public class ApiPlataformaSolis : ApiPlataforma
    {
        public ApiPlataformaSolis():base("https://www.soliscloud.com:13333", 
                                        "1300319277300421136", 
                                        "931109392bc1410fbddf7a65e365a3c6") {}

        public override async Task<decimal?[]?> mObtenGeneracionAnualInversor(int iAnio, string idInstalacionAPI)
        {
            const string rutaEspecifica = "/v1/api/stationYear";

            HttpClient httpClient = new HttpClient();

            var jsonIden = JsonConvert.SerializeObject(new { id = idInstalacionAPI, money = "MXP", year = iAnio, timezone = 8 });

            var httpRequestMessage = this.ObtenRequestMessage(jsonIden, rutaEspecifica);

            var resultReales = await httpClient.SendAsync(httpRequestMessage);

            var arrValoresReales = new decimal?[12];

            if (resultReales.IsSuccessStatusCode)
            {
                var respuesta = await resultReales.Content.ReadAsStringAsync();
                var respJson = JsonConvert.DeserializeObject<Skysense_models.APIS.APISolis>(respuesta);

                foreach (var d in respJson.data)
                {
                    var mes = DateTime.ParseExact(d.dateStr, "yyyy-MM", new CultureInfo("en-US")).Month;
                    arrValoresReales[mes - 1] = d.energy;
                }
            }
            else
            {
                arrValoresReales = null;
            }

            return arrValoresReales;
        }

        public async override Task<InversorDataDiaria[]> mObtenGeneracionDiariaInversores(string idInstalacion, DateTime fecha, params InversorInfo[] arrInversores)
        {
            InversorInfo[] arrInv = arrInversores ?? Array.Empty<InversorInfo>();

            if (arrInv.Length == 0)
            {
                arrInv = ((await mObtenListaInversores(idInstalacion))[idInstalacion] ?? []).ToArray();
            }

            return arrInv.Select(inv =>
            {
                var ret = mObtenGeneracionDiariaInversor(inv.identificador, fecha).Result;
                ret.encabezado = "Inversor " + inv.numeroSerie;
                return ret;
            }).ToArray();
        }

        private async Task<InversorDataDiaria> mObtenGeneracionDiariaInversor(string idInversor, DateTime fecha)
        {
            const string rutaEspecifica = "/v1/api/inverterMonth";

            HttpClient httpClient = new HttpClient();

            var jsonIden = JsonConvert.SerializeObject(new { id = idInversor, money = "MXP", month = fecha.ToString("yyyy-MM") });

            var httpRequestMessage = this.ObtenRequestMessage(jsonIden, rutaEspecifica);

            var resultReales = await httpClient.SendAsync(httpRequestMessage);

            var retValor = new InversorDataDiaria()
            {
                identificador = idInversor,
                numeroSerie = "",
                inversorData = []
            };

            if (resultReales.IsSuccessStatusCode)
            {
                var respuesta = await resultReales.Content.ReadAsStringAsync();
                var respJson = JsonConvert.DeserializeObject<APISolisInverterDaily>(respuesta);

                return new InversorDataDiaria()
                {
                    identificador = idInversor,
                    numeroSerie = idInversor,
                    inversorData = respJson?.data?.Select(d => new InversorData()
                    {
                        fecha = DateTime.ParseExact(d.dateStr, "yyyy-MM-dd", new CultureInfo("en-US")),
                        generacion = d.energy
                    })?.ToArray() ?? []
                };
            }

            return retValor;
        }

        private HttpRequestMessage? ObtenRequestMessage(string jsonBody, string rutaEspecifica)
        {
            HttpClient httpClient = new HttpClient();

            var jsonIden = jsonBody;
            var content = new StringContent(jsonIden);

            var md5 = Convert.ToBase64String(MD5.HashData(System.Text.Encoding.ASCII.GetBytes(jsonIden)));
            var fechaUTC = DateTime.UtcNow.ToString("ddd, d MMM yyyy HH:mm:ss 'GMT'", new CultureInfo("en-US"));

            string canonalized = "POST" + "\n" + md5 + "\napplication/json\n" + fechaUTC + "\n" + rutaEspecifica;

            var Authoriz = Convert.ToBase64String(HMACSHA1.HashData(System.Text.Encoding.ASCII.GetBytes(_apiPwd), System.Text.Encoding.ASCII.GetBytes(canonalized)));

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                Content = content
            };

            httpRequestMessage.Content.Headers.TryAddWithoutValidation("Content-MD5", md5);
            httpRequestMessage.Content.Headers.Remove("Content-Type");
            httpRequestMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json; charset=UTF-8");
            httpRequestMessage.Content.Headers.TryAddWithoutValidation("Date", fechaUTC);
            httpRequestMessage.Headers.TryAddWithoutValidation("Date", fechaUTC);
            httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "API " + _apiUser + ":" + Authoriz);

            return httpRequestMessage;
        }

        public async override Task<Dictionary<string, InversorInfo[]>> mObtenListaInversores(params string[] idInstalacionAPI)
        {
            const string rutaEspecifica = "/v1/api/inverterList";

            HttpClient httpClient = new HttpClient();

            var inversores = new Dictionary<string, InversorInfo[]>();
            int pageNo = 1;
            bool hayMasDatos = true;

            while (hayMasDatos)
            {
                string jsonIden = idInstalacionAPI.Length == 1
                    ? JsonConvert.SerializeObject(new { stationId = idInstalacionAPI.First(), pageNo = pageNo, pageSize = 100 })
                    : JsonConvert.SerializeObject(new { pageNo = pageNo, pageSize = 100 });

                var httpRequestMessage = this.ObtenRequestMessage(jsonIden, rutaEspecifica);
                var resultReales = await httpClient.SendAsync(httpRequestMessage);

                if (resultReales.IsSuccessStatusCode)
                {
                    var respuesta = await resultReales.Content.ReadAsStringAsync();
                    var respJson = JsonConvert.DeserializeObject<APISolisInverterList>(respuesta);

                    var records = respJson.data.page.records;

                    // Agrupa y agrega los inversores por estación
                    records.GroupBy(r => r.stationId).Where(r => idInstalacionAPI.Contains(r.Key)).ToList().ForEach(station =>
                    {
                        if (!inversores.ContainsKey(station.Key))
                            inversores[station.Key] = station.Select((s, i) => new InversorInfo()
                            {
                                idConsecutivo = i + ((pageNo - 1) * 100),
                                numeroSerie = s.sn,
                                fechaInicio = MetodosComunes.UnixTimeStampToDateTime(s.fisGenerateTime),
                                identificador = s.id
                            }).ToArray();
                        else
                            inversores[station.Key] = inversores[station.Key].Concat(
                                station.Select((s, i) => new InversorInfo()
                                {
                                    idConsecutivo = inversores[station.Key].Length + i,
                                    numeroSerie = s.sn,
                                    fechaInicio = MetodosComunes.UnixTimeStampToDateTime(s.fisGenerateTime),
                                    identificador = s.id
                                })
                            ).ToArray();
                    });

                    // Si el número de registros recibidos es menor al pageSize, ya no hay más datos
                    hayMasDatos = respJson.data.page.total > (pageNo * 100);
                    pageNo++;
                }
                else
                {
                    hayMasDatos = false;
                }
            }

            return inversores;
        }

        public async override Task<InversorDataDiaria[]> mMonitoreaGeneracionTiempoReal(string idInstalacionAPI, params InversorInfo[] arrInversores)
        {
            throw new NotImplementedException();

            //const string rutaEspecifica = "/v1/api/inverterDay";
            //const int tiempoEspera = 1;

            //HttpClient httpClient = new HttpClient();

            //var fechaActual = DateTime.Now;

            ////IMPORTANT: COMPROBAR SI TODO SE EJECUTA
            //for(int i = 0; i < arrInversores.Length; i++)
            //{
            //    var jsonIden = JsonConvert.SerializeObject(new { id= arrInversores[i].identificador, money = "MXP", time = fechaActual.ToString("yyyy-MM-dd"), timeZone = 8 });
            //    var httpRequestMessage = this.ObtenRequestMessage(jsonIden, rutaEspecifica);

            //    var resultReales = await httpClient.SendAsync(httpRequestMessage);

            //    if (resultReales.IsSuccessStatusCode)
            //    {
            //        var respuesta = await resultReales.Content.ReadAsStringAsync();
            //        var respJson = JsonConvert.DeserializeObject<APISolisInverterDaily>(respuesta);

            //        return new InversorDataDiaria()
            //        {
            //            identificador = idInversor,
            //            numeroSerie = idInversor,
            //            inversorData = respJson?.data?.Select(d => new InversorData()
            //            {
            //                fecha = DateTime.ParseExact(d.dateStr, "yyyy-MM-dd", new CultureInfo("en-US")),
            //                generacion = d.energy
            //            })?.ToArray() ?? []
            //        };
            //    }
            //}


            //var retValor = new InversorDataDiaria()
            //{
            //    identificador = idInversor,
            //    numeroSerie = "",
            //    inversorData = []
            //};

            

            //return retValor;
        }

    }

    public class ApiPlataformaFusionSolar : ApiPlataforma
    {
        private static DateTime _ExpiracionToken = DateTime.Now;
        private static string _Token = "";
        private async Task<string> GetToken()
        {
            bool tokenValido = true;

            //Si el token ya no es válido
            if (_ExpiracionToken < DateTime.Now)
            {
                tokenValido = await this.mObtenToken();
            }

            //Si el token NO se actualizó
            if (!tokenValido)
            {
                throw new AuthenticationException("Ocurrió un problema con la autenticación de la plataforma Fusion Solar.");
            }

            return await Task.FromResult(_Token);
        }

        public ApiPlataformaFusionSolar() : base("https://la5.fusionsolar.huawei.com/thirdData",
                                        "SkysenseAPP",
                                        "Skysense1")
        { }

        private async Task<bool> mObtenToken()
        {
            bool respuesta = false;

            const string rutaEspecifica = "/login";

            HttpClient httpClient = new HttpClient();
            var jsonIden = JsonConvert.SerializeObject(new { userName = _apiUser, systemCode = _apiPwd });
            var content = new StringContent(jsonIden, null, "application/json");

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                Content = content
            };

            var resp = await httpClient.SendAsync(httpRequestMessage);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var stream = await resp.Content.ReadAsStringAsync();
                var contenido = JsonConvert.DeserializeObject<dynamic>(stream);

                if(contenido.success == true)
                {
                    _Token = resp.Headers.GetValues("xsrf-token")?.First();
                    _ExpiracionToken = DateTime.Now.AddMinutes(29);
                    respuesta = true;
                }
            }

            return respuesta;
        }

        public override async Task<decimal?[]?> mObtenGeneracionAnualInversor(int iAnio, string idInstalacionAPI)
        {
            const string rutaEspecifica = "/getKpiStationMonth";

            HttpClient httpClient = new HttpClient();

            var jsonIden = JsonConvert.SerializeObject(new { stationCodes = idInstalacionAPI, collectTime = MetodosComunes.DateTimeToUnixTimeStampMS(new DateTime(iAnio, 1, 10)) });

            var httpRequestMessage = await ObtenRequestMessage(jsonIden, rutaEspecifica);
            var resultReales = await httpClient.SendAsync(httpRequestMessage);

            var arrValoresReales = new decimal?[12];

            if (resultReales.IsSuccessStatusCode)
            {
                var respuesta = await resultReales.Content.ReadAsStringAsync();
                var respJson = JsonConvert.DeserializeObject<APIFusionSolar<APIEnergiaDataFusionSolar>>(respuesta);

                if (respJson.failCode == 305)
                {
                    await this.mObtenToken();
                    return null;
                }

                foreach (var d in respJson.data)
                {

                    var mes = DateTime.UnixEpoch.AddMilliseconds(d.collectTime).ToUniversalTime();//.Month;
                    arrValoresReales[mes.Month - 1] = d.dataItemMap.inverterYield;
                }
            }
            else
            {
                arrValoresReales = null;
            }

            return arrValoresReales;
        }

        public override async Task<Dictionary<string, InversorInfo[]>> mObtenListaInversores(params string[] idInstalacionAPI)
        {
            const string rutaEspecifica = "/getDevList";

            HttpClient httpClient = new HttpClient();

            var jsonIden = JsonConvert.SerializeObject(new { stationCodes = string.Join(",", idInstalacionAPI) });

            var httpRequestMessage = await ObtenRequestMessage(jsonIden, rutaEspecifica);
            var resultReales = await httpClient.SendAsync(httpRequestMessage);

            Dictionary<string, InversorInfo[]> arrInversores = new Dictionary<string, InversorInfo[]>();

            if (resultReales.IsSuccessStatusCode)
            {
                var respuesta = await resultReales.Content.ReadAsStringAsync();
                var respJson = JsonConvert.DeserializeObject<APIFusionSolar<APIFusionSolar_Devices>>(respuesta);

                //Este código es de token expirado
                if (respJson.failCode == 305)
                {
                    _ExpiracionToken = DateTime.Now;
                }
                else if (respJson.success)
                {

                    int idConsecutivo = 1;

                    respJson.data.GroupBy(d => d.stationCode).ToList().ForEach(station =>
                    {
                        arrInversores.Add(station.Key, station.Where(d => d.devTypeId == 38 || d.devTypeId == 1).Select(d => new InversorInfo()
                        {
                            numeroSerie = d.esnCode,
                            idConsecutivo = idConsecutivo++,
                            identificador = $"{d.id};{d.devTypeId}"
                        }).ToArray());
                    });
                }
            }

            return arrInversores;
        }

        public override async Task<InversorDataDiaria[]> mObtenGeneracionDiariaInversores(string idInstalacionAPI, DateTime fecha, params InversorInfo[] arrInversores)
        {
            const string rutaEspecifica = "/getDevKpiDay";

            var arrInv = arrInversores ?? Array.Empty<InversorInfo>();

            if (arrInv.Length == 0)
            {
                arrInv = (await mObtenListaInversores(idInstalacionAPI))[idInstalacionAPI];
            }

            List<InversorDataDiaria> lstInversores = new List<InversorDataDiaria>();

            foreach (var devType in arrInv.GroupBy(i => i.identificador.Split(";").Last()))
            {
                HttpClient httpClient = new HttpClient();

                var jsonIden = JsonConvert.SerializeObject(new
                {
                    devIds = string.Join(",", devType.Select(d=>d.identificador.Split(";").First())),
                    devTypeId = int.Parse(devType.Key),
                    collectTime = MetodosComunes.DateTimeToUnixTimeStampMS(fecha)
                });

                var httpRequestMessage = await ObtenRequestMessage(jsonIden, rutaEspecifica);
                var resultReales = await httpClient.SendAsync(httpRequestMessage);


                if (resultReales.IsSuccessStatusCode)
                {
                    var respuesta = await resultReales.Content.ReadAsStringAsync();
                    var respJson = JsonConvert.DeserializeObject<APIFusionSolar<APIFusionSolar_DevicesData>>(respuesta);

                    //Este código es de token expirado
                    if (respJson.failCode == 305)
                    {
                        _ExpiracionToken = DateTime.Now;
                    }
                    else if (respJson.success)
                    {
                        int idConsecutivo = 1;

                        foreach (var item in respJson.data.GroupBy(g => g.devId))
                        {
                            lstInversores.Add(new InversorDataDiaria()
                            {
                                encabezado = "Inversor " + item.First().sn,
                                identificador = $"{item.Key};{devType.Key}",
                                numeroSerie = item.First().sn,
                                inversorData = item.Select(i => new InversorData()
                                {
                                    fecha = MetodosComunes.UnixTimeStampToDateTime(i.collectTime),
                                    generacion = i.dataItemMap.product_power ?? 0
                                }).ToArray()
                            });
                        }
                    }
                }
            }

            return lstInversores.ToArray();
        }

        private async Task<HttpRequestMessage?> ObtenRequestMessage(string jsonBody, string rutaEspecifica)
        {
            HttpClient httpClient = new HttpClient();

            var content = new StringContent(jsonBody, null, "application/json");

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                Content = content
            };

            httpRequestMessage.Headers.TryAddWithoutValidation("xsrf-token", await this.GetToken());

            return httpRequestMessage;
        }

        public override Task<InversorDataDiaria[]> mMonitoreaGeneracionTiempoReal(string idInstalacionAPI, params InversorInfo[] arrInversores)
        {
            throw new NotImplementedException();
        }
    }

    public class ApiPlataformaSungrow : ApiPlataforma
    {
        private readonly string _Cuenta = "monitoreo@skysense.com.mx";
        private readonly string _Pwd = "123456789";
        private static string _Token = "";
        private static DateTime _TokenExpiracy = DateTime.Now;
        private async Task<string> GetToken()
        {
            if (_TokenExpiracy <= DateTime.Now)
            {
                if (!await this.mObtenToken())
                {
                    _TokenExpiracy = DateTime.Now;
                    throw new AuthenticationException("Error al obtener el token.");
                }
            }

            return _Token;
        }

        public ApiPlataformaSungrow() : base("https://gateway.isolarcloud.com.hk",
                                        "A710A8D9DEA60581F9AE9F5A8F308822",
                                        "63sjnxi9pbyip6xm8rzfew7y9d30iem1")
        { }

        private async Task<bool> mObtenToken()
        {
            bool respuesta = false;

            const string rutaEspecifica = "/openapi/login";

            HttpClient httpClient = new HttpClient();
            var jsonIden = JsonConvert.SerializeObject(new { appkey = _apiUser, user_account = _Cuenta, user_password = _Pwd});
            var content = new StringContent(jsonIden, null, "application/json");

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                Content = content
            };

            httpRequestMessage.Headers.TryAddWithoutValidation("sys_code", "901");
            httpRequestMessage.Headers.TryAddWithoutValidation("x-access-key", _apiPwd);

            var resp = await httpClient.SendAsync(httpRequestMessage);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var stream = await resp.Content.ReadAsStringAsync();
                var contenido = JsonConvert.DeserializeObject<APISungrow_Token>(stream)!;

                if (contenido.result_code == "1")
                {
                    _Token = contenido.result_data.token;
                    _TokenExpiracy = DateTime.Now.AddHours(22);
                    respuesta = true;
                }
            }

            return respuesta;
        }

        public override async Task<decimal?[]?> mObtenGeneracionAnualInversor(int iAnio, string idInstalacionAPI)
        {
            const string rutaEspecifica = "/openapi/getDevicePointsDayMonthYearDataList";

            var listaInversores = (await this.mObtenListaInversores(idInstalacionAPI))[idInstalacionAPI] ?? [];

            HttpClient httpClient = new HttpClient();
            var jsonIden = JsonConvert.SerializeObject(new { appkey = _apiUser, token = await GetToken(), data_point = "p1",
                start_time = iAnio.ToString() + "01",end_time = iAnio.ToString() + "12",
	            query_type="2", ps_key_list = listaInversores.Select(l=>l.identificador).ToArray(), data_type="4",order="0" });

            var content = new StringContent(jsonIden, null, "application/json");

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                Content = content
            };

            httpRequestMessage.Headers.TryAddWithoutValidation("sys_code", "901");
            httpRequestMessage.Headers.TryAddWithoutValidation("x-access-key", _apiPwd);

            var energiaPorMes = new decimal[12];

            var resp = await httpClient.SendAsync(httpRequestMessage);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var stream = await resp.Content.ReadAsStringAsync();
                var contenido = JsonConvert.DeserializeObject<APISungrow_InvertersYield<APISungrow_InvertersYieldInfo4>>(stream)!;
                if (contenido.result_code == "1")
                {
                    foreach (var inversor in contenido.result_data.Values)
                    {
                        for (int i = 0; i < energiaPorMes.Length; i++)
                        {
                            energiaPorMes[i] += inversor.p1
                                .Where(p => p.time_stamp == iAnio.ToString() + (i+1).ToString("00"))
                                .Sum(p => p.dato);
                        }
                    }
                }
            }

            return energiaPorMes.Select(d=>(decimal?)d/1000).ToArray();
        }

        public override async Task<Dictionary<string, InversorInfo[]>> mObtenListaInversores(params string[] pidInstalacionAPI)
        {
            const string rutaEspecifica = "/openapi/getDeviceList";

            Dictionary<string, InversorInfo[]> listaInversores = new Dictionary<string, InversorInfo[]>();  

            foreach (var idInstalacionAPI in pidInstalacionAPI)
            {
                HttpClient httpClient = new HttpClient();
                var jsonIden = JsonConvert.SerializeObject(new { appkey = _apiUser, token = await GetToken(), ps_id = idInstalacionAPI, curPage = 1, size = 100, device_type_list = new[] { 1 } });
                var content = new StringContent(jsonIden, null, "application/json");

                var httpRequestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                    Content = content
                };

                httpRequestMessage.Headers.TryAddWithoutValidation("sys_code", "901");
                httpRequestMessage.Headers.TryAddWithoutValidation("x-access-key", _apiPwd);

                var resp = await httpClient.SendAsync(httpRequestMessage);


                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    var stream = await resp.Content.ReadAsStringAsync();
                    var contenido = JsonConvert.DeserializeObject<APISungrow_Device>(stream)!;
                    if (contenido.result_code == "1")
                    {
                        var i = 0;

                        listaInversores.Add(idInstalacionAPI, contenido.result_data.pageList.Select(d => new InversorInfo()
                        {
                            numeroSerie = d.device_sn,
                            fechaInicio = d.grid_connection_date,
                            idConsecutivo = i++,
                            identificador = d.ps_key
                        }).ToArray());
                    }
                }
            }

            return listaInversores;
        }

        public override async Task<InversorDataDiaria[]> mObtenGeneracionDiariaInversores(string idInstalacionAPI, DateTime fecha, params InversorInfo[] arrInversores)
        {
            const string rutaEspecifica = "/openapi/getDevicePointsDayMonthYearDataList";
            const int limiteDevices = 50;

            var arrInv = arrInversores ?? Array.Empty<InversorInfo>();
            var listaInversores = new List<InversorInfo>();

            if (arrInv.Length == 0)
            {
                listaInversores = ((await mObtenListaInversores(idInstalacionAPI))[idInstalacionAPI] ?? []).ToList();
                arrInv = listaInversores.ToArray();
            }
            else
            {
                listaInversores = arrInv.ToList();
            }

            var inversoresData = new List<InversorDataDiaria>();

            // Procesar en lotes de 50 inversores
            for (int i = 0; i < arrInv.Length; i += limiteDevices)
            {
                var lote = arrInv.Skip(i).Take(limiteDevices).Select(i=>i.identificador).ToArray();

                HttpClient httpClient = new HttpClient();
                var jsonIden = JsonConvert.SerializeObject(new
                {
                    appkey = _apiUser,
                    token = await GetToken(),
                    data_point = "p1",
                    start_time = fecha.ToString("yyyyMM") + "01",
                    end_time = fecha.ToString("yyyyMM") + fecha.AddDays(-(fecha.Day - 1)).AddMonths(1).AddDays(-1).Day,
                    query_type = "1",
                    ps_key_list = lote,
                    data_type = "2",
                    order = "0"
                });

                var content = new StringContent(jsonIden, null, "application/json");

                var httpRequestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_apiUrlBase + rutaEspecifica),
                    Content = content
                };

                httpRequestMessage.Headers.TryAddWithoutValidation("sys_code", "901");
                httpRequestMessage.Headers.TryAddWithoutValidation("x-access-key", _apiPwd);

                var resp = await httpClient.SendAsync(httpRequestMessage);

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    var stream = await resp.Content.ReadAsStringAsync();
                    var contenido = JsonConvert.DeserializeObject<APISungrow_InvertersYield<APISungrow_InvertersYieldInfo2>>(stream)!;
                    if (contenido.result_code == "1")
                    {
                        foreach (var item in contenido.result_data)
                        {
                            inversoresData.Add(new InversorDataDiaria()
                            {
                                encabezado = "Inversor " + item.Key,
                                identificador = item.Key,
                                numeroSerie = listaInversores.FirstOrDefault(inv => inv.identificador == item.Key)?.numeroSerie ?? "",
                                inversorData = item.Value.p1.Select(i => new InversorData
                                {
                                    generacion = i.dato / 1000,
                                    fecha = DateTime.ParseExact(i.time_stamp, "yyyyMMdd", CultureInfo.InvariantCulture)
                                }).ToArray()
                            });
                        }
                    }
                }
            }

            return inversoresData.ToArray();
        }

        public override Task<InversorDataDiaria[]> mMonitoreaGeneracionTiempoReal(string idInstalacionAPI, params InversorInfo[] arrInversores)
        {
            throw new NotImplementedException();
        }
    }

}
