IF EXISTS(SELECT [object_id] FROM sys.objects WHERE [name]='ExtractDateFromFileName' AND [type]='FN' AND [schema_id]=SCHEMA_ID('Submissions'))
BEGIN
    DROP FUNCTION Submissions.ExtractDateFromFileName
END
GO

CREATE FUNCTION Submissions.ExtractDateFromFileName
(
	@Filename nvarchar(50)
)
RETURNS datetime
AS
BEGIN
	DECLARE @FileDateStr varchar(8) = LEFT(RIGHT(@FileName,22),8)
	IF(LEN(@FileDateStr)<8)
		BEGIN
			RETURN null
		END

	DECLARE @year char(4) = LEFT(@FileDateStr,4)
	DECLARE @month char(2) = SUBSTRING(@FileDateStr,5,2)
	DECLARE @day char(2) = RIGHT(@FileDateStr,2)
	RETURN DATEFROMPARTS(@year,@month,@day)
END
GO