USE [EmployeePerformance]
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_GetPipSummaryReport]
    @ManagerID INT = NULL,
    @Status    NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pp.PIPID                                                    AS PipId,
        emp.FirstName + ' ' + emp.LastName                         AS EmployeeName,
        mgr.FirstName + ' ' + mgr.LastName                         AS ManagerName,
        pp.Status,
        pp.StartDate,
        pp.EndDate,
        pp.OverallGoal,
        COUNT(DISTINCT po.ObjectiveID)                             AS TotalObjectives,
        SUM(CASE WHEN po.IsAchieved = 1 THEN 1 ELSE 0 END)        AS AchievedObjectives,
        COUNT(DISTINCT pm.PIP_MeetingID)                           AS TotalMeetings
    FROM dbo.PIP_Plans pp
    INNER JOIN dbo.Employees emp ON pp.EmployeeID = emp.EmployeeID
    INNER JOIN dbo.Employees mgr ON pp.ManagerID  = mgr.EmployeeID
    LEFT  JOIN dbo.PIP_Objectives po ON pp.PIPID = po.PIPID
    LEFT  JOIN dbo.PIP_Meetings   pm ON pp.PIPID = pm.PIPID
    WHERE
        (@ManagerID IS NULL OR pp.ManagerID = @ManagerID)
        AND (@Status IS NULL OR pp.Status = @Status)
    GROUP BY
        pp.PIPID, emp.FirstName, emp.LastName,
        mgr.FirstName, mgr.LastName,
        pp.Status, pp.StartDate, pp.EndDate, pp.OverallGoal
    ORDER BY pp.CreatedAt DESC;
END
GO