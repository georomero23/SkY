Create Table Bess(
	IdInstalacion int FOREIGN KEY REFERENCES Instalaciones(IdInstalacion) PRIMARY KEY,
	UrlConexionBess varchar(100),
	CadenaConexion varchar(MAX),
	Monitorear bit NOT NULL,
	Estatus tinyint,
	UltimaActualizacionEstatus Datetime2,
	MensajeEstatus varchar(200)
)

CREATE TABLE ParametrosBess(
	IdParametro smallint NOT NULL PRIMARY KEY,
	Medicion varchar(50) NOT NULL,
	Unidad varchar(20),
	SoloLectura bit NOT NULL DEFAULT 1,
	Observaciones varchar(200),
	EsBooleano bit NOT NULL,
	EsGraficable bit NOT NULL
)

CREATE TABLE TagsBess(
	IdTag int NOT NULL PRIMARY KEY Identity(1,1),
	IdBess int NOT NULL FOREIGN KEY REFERENCES Bess(IdInstalacion),
	IdParametro smallint NOT NULL FOREIGN KEY REFERENCES ParametrosBess(IdParametro),
	Tag varchar(50) NOT NULL,
	Monitorear bit NOT NULL,
	Estatus tinyint,
	UltimaActualizacionEstatus Datetime2,
	MensajeEstatus varchar(200),
	Activo bit NOT NULL,
	NombreAMostrar varchar(100),
	IdGrafica tinyint
)

CREATE TABLE MedicionesBess(
	IdMedicion int NOT NULL Identity(-2147483648,1) Primary KEY,
	IdTag int NOT NULL,
	FechaMedicion datetime2 NOT NULL,
	ValorMedicion int NOT NULL
)

CREATE TABLE BessAlertas(
	IdAlerta int NOT NULL PRIMARY KEY Identity(1,1),
	IdBESS int NOT NULL FOREIGN KEY REFERENCES BESS(IdInstalacion),
	IdNivelAlerta tinyint NOT NULL DEFAULT 1,
	Titulo varchar(50) NOT NULL,
	Texto varchar(500) NOT NULL,
	FechaCreacion datetime2 NOT NULL,
	FechaActualizacion datetime2,
	IdEstado tinyint NOT NULL
)

CREATE TABLE ParametrosSistema(
	IdParametro int NOT NULL PRIMARY KEY,
	NombreParametro varchar(50) UNIQUE NOT NULL,
	Descripcion varchar(200),
	Valor varchar(MAX) NOT NULL,
	TipoDato varchar(1) NOT NULL
)

INSERT INTO ParametrosBess
VALUES
(1,'Potencia Activa', 'kW',''),
(2,'Potencia Reactiva', 'kVar',''),
(3,'Factor de Potencia', 'N/A','')

INSERT INTO CatalogoMaestro VALUES
(14, 'Estado Monitoreo EMS y TAGS', 'Catálogo de los estados que puede tener los EMS y los TAGS al monitorearlos.', 0),
(15, 'Niveles Alerta', 'Niveles de alertas.', 0),
(16, 'Estado de Alertas', 'Estado de alertas.', 0)

INSERT INTO CatalogoOpciones VALUES
(14, 1, 'Sin monitoreo', '', 1),
(14, 2, 'Monitoreando', '', 1),
(14, 3, 'Error al monitorear', '', 1),
(14, 4, 'Monitoreando con errores', '', 1),
(14, 5, 'Error de configuración', '', 1),

(15, 1, 'Informativo', '', 1),
(15, 2, 'Bajo', '', 1),
(15, 3, 'Medio', '', 1),
(15, 4, 'Alto', '', 1),

(16, 1, 'Activo', '', 1),
(16, 2, 'Terminado', '', 1),
(16, 3, 'Eliminado', '', 1)

INSERT INTO ParametrosSistema VALUES
(1, 'Plantilla Cadena Conexion BESS', 'Plantilla para conectarse a la base de datos donde se almacenan los datos obtenidos para los BESS.', 'Data Source=SKY-C4MXLP16\SQLEXPRESS;Initial Catalog={0};User ID=sa;Password=Skysense2025;Persist Security Info=True;Trust Server Certificate=True;MultipleActiveResultSets=true', 'T');
