CREATE OR ALTER PROCEDURE spGeneraBDDMediciones (
	@idInstalacion int,
	@vcNuevaBD varchar(100) output,
	@bError bit output,
	@sMensajeInfo nvarchar(max) output
)
As 
BEGIN

	declare @CreateTableSQL nvarchar(max);
	SET @bError = 0;
	SET @sMensajeInfo = 'Inicio de creación de base de datos...'

	IF NOT EXISTS (SELECT 1 FROM Instalaciones 
				WHERE IdInstalacion = @idInstalacion AND 
				(TipoProyecto = 2 OR TipoProyecto = 3))
	BEGIN
		Set @bError = 1
		SET @sMensajeInfo = 'La instalación especificada no es de BESS.'
		RETURN
	END

	DECLARE @CreateDBSQL NVARCHAR(MAX);
	declare @nuevaDB varchar(50) = 'Skysense_Inst_'+CONVERT(varchar(10),@idInstalacion)
	SET @vcNuevaBD = @nuevaDB;
	--Se verifica que esa base de datos no exista
	IF EXISTS (SELECT name FROM master.sys.databases WHERE name = @nuevaDB)
	BEGIN
		Set @bError = 1
		SET @sMensajeInfo = 'Ya existe una base de datos para la instalación '+CONVERT(varchar(10),@idInstalacion)+'.'
		Return
	END

	BEGIN TRY
		--Se crea la base de datos
		SET @CreateDBSQL = N'CREATE DATABASE ' + QUOTENAME(@nuevaDB) + ';';
		EXEC sp_executesql @CreateDBSQL;


		SET @CreateTableSQL = N'Use '+ QUOTENAME(@nuevaDB)+';'+
		N'CREATE TABLE MedicionesBess(
									IdMedicion int NOT NULL Identity(-2147483648,1) Primary KEY,
									IdTag int NOT NULL,
									FechaMedicion datetime2 NOT NULL,
									ValorMedicion int NOT NULL
								);';

		EXEC(@CreateTableSQL);


		declare @cadena varchar(500) = (Select Valor FROM ParametrosSistema WHERE IdParametro = 1)
		SET @vcNuevaBD = (SELECT REPLACE(@cadena,'{0}', @vcNuevaBD));
		SET @vcNuevaBD = ( SELECT CAST(@vcNuevaBD AS VARBINARY(MAX)) FOR XML PATH(''), BINARY BASE64);

	END TRY
	BEGIN CATCH
		--Como es una creación de base de datos, no se puede hacer rollback. Entonces mejor la eliminamos manualmente.
		declare @deleteDB nvarchar(1000) = N'DROP DATABASE IF EXISTS ' + QUOTENAME(@nuevaDB) + ';';
		EXEC sp_executesql @deleteDB;
		--REVOKE ALL PRIVILEGES ON your_database_name.* FROM 'your_user'@'localhost';

		Set @bError = 1
		SET @sMensajeInfo = 'Error creando la nueva base de datos: ' + ERROR_MESSAGE()
		RETURN

	END CATCH

	SET @sMensajeInfo = 'Creación de la base de datos y tabla correcta.';

END


