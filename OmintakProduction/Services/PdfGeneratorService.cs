using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Geom;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using OmintakProduction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OmintakProduction.Services
{
    public class PdfGeneratorService
    {
        public byte[] GenerateBugReportPdf(IEnumerable<BugReport> bugReports)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Apply document settings
                PdfStyleService.ApplyDocumentSettings(document);

                // Add title
                var title = new Paragraph("Bug Report");
                PdfStyleService.ApplyHeaderStyle(title);
                document.Add(title);

                // Add timestamp
                var timestamp = new Paragraph($"Generated on: {DateTime.Now}");
                PdfStyleService.ApplySubHeaderStyle(timestamp);
                document.Add(timestamp);

                // Create table with styling
                Table table = PdfStyleService.CreateStandardTable(6);

                // Add headers with styling
                string[] headers = { "Title", "Description", "Status", "Severity", "Reported By", "Created At" };
                // Add header cells
                foreach (var header in headers)
                {
                    var cell = PdfStyleService.CreateStyledCell(header)
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                    table.AddHeaderCell(cell);
                }

                // Add data with alternating row styling
                bool alternate = false;
                foreach (var bug in bugReports)
                {
                    Cell[] cells = {
                        new Cell().Add(new Paragraph(bug.Title)),
                        new Cell().Add(new Paragraph(bug.Description ?? "")),
                        new Cell().Add(new Paragraph(bug.Status.ToString())),
                        new Cell().Add(new Paragraph(bug.Severity.ToString())),
                        new Cell().Add(new Paragraph(bug.ReportedByUser?.UserName ?? "Unknown")),
                        new Cell().Add(new Paragraph(bug.CreatedAt.ToString("MMM dd, yyyy")))
                    };

                    foreach (var cell in cells)
                    {
                        if (alternate)
                        {
                            cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                        }
                        table.AddCell(cell);
                    }
                    alternate = !alternate;
                }

                document.Add(table);
                document.Close();

                return ms.ToArray();
            }
        }

        public byte[] GenerateProjectReportPdf(Project project)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Add title
                var title = new Paragraph("Project Report");
                PdfStyleService.ApplyHeaderStyle(title);
                document.Add(title);

                // Project Details
                var projectName = new Paragraph($"Project: {project.ProjectName}");
                PdfStyleService.ApplySubHeaderStyle(projectName);
                document.Add(projectName);

                // Project Information
                var details = new Table(2).UseAllAvailableWidth();
                PdfStyleService.ApplyTableHeaderStyle(details);

                details.AddCell(PdfStyleService.CreateStyledCell("Description:", TextAlignment.RIGHT));
                details.AddCell(PdfStyleService.CreateStyledCell(project.Description));
                
                details.AddCell(PdfStyleService.CreateStyledCell("Status:", TextAlignment.RIGHT));
                var statusCell = PdfStyleService.CreateStyledCell(project.Status);
                PdfStyleService.ApplyStatusStyle(statusCell, project.Status);
                details.AddCell(statusCell);
                
                details.AddCell(PdfStyleService.CreateStyledCell("Due Date:", TextAlignment.RIGHT));
                details.AddCell(PdfStyleService.CreateStyledCell(project.DueDate.ToString("MMM dd, yyyy")));

                document.Add(details);

                // Task Summary
                var taskTitle = new Paragraph("Tasks");
                PdfStyleService.ApplySubHeaderStyle(taskTitle);
                document.Add(taskTitle);

                Table taskTable = new Table(4).UseAllAvailableWidth();
                
                // Add headers
                taskTable.AddHeaderCell("Task");
                taskTable.AddHeaderCell("Status");
                taskTable.AddHeaderCell("Assigned To");
                taskTable.AddHeaderCell("Due Date");
                
                PdfStyleService.ApplyTableHeaderStyle(taskTable);

                bool alternate = false;
                foreach (var task in project.Tasks)
                {
                    var titleCell = PdfStyleService.CreateStyledCell(task.Title);
                    var taskStatusCell = PdfStyleService.CreateStyledCell(task.Status.ToString(), TextAlignment.CENTER);
                    PdfStyleService.ApplyStatusStyle(taskStatusCell, task.Status.ToString());
                    
                    // Get first assigned user or "Unassigned"
                    string assignedTo = task.AssignedUsers.FirstOrDefault()?.UserName ?? "Unassigned";
                    var assignedToCell = PdfStyleService.CreateStyledCell(assignedTo);
                    
                    string dueDate = task.DueDate.HasValue ? task.DueDate.Value.ToString("MMM dd, yyyy") : "No due date";
                    var dueDateCell = PdfStyleService.CreateStyledCell(dueDate, TextAlignment.CENTER);

                    Cell[] cells = { titleCell, taskStatusCell, assignedToCell, dueDateCell };
                    foreach (var cell in cells)
                    {
                        taskTable.AddCell(cell);
                    }
                    alternate = !alternate;
                }

                document.Add(taskTable);

                // Add footer with datetime
                PdfStyleService.AddDateTimeFooter(document);

                document.Close();
                return ms.ToArray();
            }
        }

        public byte[] GenerateDashboardReportPdf(
            IEnumerable<Project> projects, 
            IEnumerable<BugReport> bugReports, 
            IEnumerable<Models.Task> tasks)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Dashboard Overview
                var title = new Paragraph("Dashboard Report");
                PdfStyleService.ApplyHeaderStyle(title);
                document.Add(title);

                // Statistics Grid - Create a 3-column layout for stats
                Table statsGrid = new Table(3).UseAllAvailableWidth();

                // Projects Statistics
                var projectsTitle = new Paragraph("Projects Overview");
                PdfStyleService.ApplySubHeaderStyle(projectsTitle);
                document.Add(projectsTitle);

                var activeProjects = projects.Count(p => p.Status == "Active");
                var completedProjects = projects.Count(p => p.Status == "Done");
                
                PdfStyleService.CreateStatisticBlock(document, "Active Projects", activeProjects.ToString());
                PdfStyleService.CreateStatisticBlock(document, "Completed Projects", completedProjects.ToString());

                // Tasks Statistics
                var tasksTitle = new Paragraph("Tasks Overview");
                PdfStyleService.ApplySubHeaderStyle(tasksTitle);
                document.Add(tasksTitle);

                var pendingTasks = tasks.Count(t => t.Status == Models.TaskStatus.Todo);
                var inProgressTasks = tasks.Count(t => t.Status == Models.TaskStatus.InProgress);
                var completedTasks = tasks.Count(t => t.Status == Models.TaskStatus.Completed);

                PdfStyleService.CreateStatisticBlock(document, "Pending Tasks", pendingTasks.ToString());
                PdfStyleService.CreateStatisticBlock(document, "In Progress Tasks", inProgressTasks.ToString());
                PdfStyleService.CreateStatisticBlock(document, "Completed Tasks", completedTasks.ToString());

                // Bug Reports Statistics
                var bugsTitle = new Paragraph("Bug Reports Overview");
                PdfStyleService.ApplySubHeaderStyle(bugsTitle);
                document.Add(bugsTitle);

                var openBugs = bugReports.Count(b => b.Status == BugStatus.Open);
                var inProgressBugs = bugReports.Count(b => b.Status == BugStatus.InProgress);
                var resolvedBugs = bugReports.Count(b => b.Status == BugStatus.Resolved);

                PdfStyleService.CreateStatisticBlock(document, "Open Bugs", openBugs.ToString());
                PdfStyleService.CreateStatisticBlock(document, "In Progress Bugs", inProgressBugs.ToString());
                PdfStyleService.CreateStatisticBlock(document, "Resolved Bugs", resolvedBugs.ToString());

                // Add footer with datetime
                PdfStyleService.AddDateTimeFooter(document);

                document.Close();
                return ms.ToArray();
            }
        }
    }
}
