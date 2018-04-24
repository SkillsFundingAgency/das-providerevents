
WITH dates AS 
(
	SELECT CAST('2016-08-01' AS date) AS DateValue
    UNION ALL
    SELECT DATEADD(MONTH, 1, DateValue) AS NextDate
    FROM dates AS dates2
    WHERE DATEADD(MONTH, 1, DateValue) < '2020-07-01'
)
INSERT [Payments].[Periods] 
(
	[PeriodName], 
	[CalendarMonth], 
	[CalendarYear], 
	[CompletionDateTime]
)
SELECT
	concat (
		datepart(year, dateadd(month, -7, DateValue)) - 2000,
		datepart(year, dateadd(month, -7, DateValue)) - 1999,
		'-R',
		format(datepart(month, DateValue) + (case when datepart(month, DateValue) < 8 then 5 else -7 end), 'd2')
	) as [PeriodName],
	datepart(month, DateValue) as [CalendarMonth],
	datepart(year, DateValue) as [CalendarYear],
	DateValue as [CompletionDateTime]
FROM 
	dates AS dates2
