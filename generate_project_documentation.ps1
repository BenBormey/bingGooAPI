$ErrorActionPreference = "Stop"

$projectRoot = "E:\Setec\MVC\bingGooAPI"
$apiRoot = Join-Path $projectRoot "bingGooAPI"
$outputDocx = Join-Path $projectRoot "Project_Documentation.docx"
$outputMd = Join-Path $projectRoot "Project_Documentation.md"

function Escape-Xml {
    param([string]$Text)
    if ($null -eq $Text) { return "" }
    return [System.Security.SecurityElement]::Escape($Text)
}

function New-ParagraphXml {
    param(
        [string]$Text,
        [string]$Style = "Normal",
        [string]$Justification = "left",
        [switch]$PageBreakBefore
    )

    $escaped = Escape-Xml $Text
    $runText = $escaped -replace "`r`n|`n|`r", "</w:t><w:br/><w:t xml:space=`"preserve`">"
    $pPr = "<w:pPr><w:pStyle w:val=`"$Style`"/><w:jc w:val=`"$Justification`"/></w:pPr>"
    if ($PageBreakBefore) {
        $pPr = "<w:pPr><w:pStyle w:val=`"$Style`"/><w:pageBreakBefore/><w:jc w:val=`"$Justification`"/></w:pPr>"
    }

    return "<w:p>$pPr<w:r><w:t xml:space=`"preserve`">$runText</w:t></w:r></w:p>"
}

function New-HeadingXml {
    param([int]$Level, [string]$Text)
    return New-ParagraphXml -Text $Text -Style "Heading$Level"
}

function New-CodeXml {
    param([string]$Text)
    return New-ParagraphXml -Text $Text -Style "Code"
}

function New-PageBreakXml {
    return "<w:p><w:r><w:br w:type=`"page`"/></w:r></w:p>"
}

function New-TocXml {
    return @"
<w:p>
  <w:r><w:fldChar w:fldCharType="begin"/></w:r>
  <w:r><w:instrText xml:space="preserve"> TOC \o "1-3" \h \z \u </w:instrText></w:r>
  <w:r><w:fldChar w:fldCharType="separate"/></w:r>
  <w:r><w:t>Right-click and update the table of contents in Word.</w:t></w:r>
  <w:r><w:fldChar w:fldCharType="end"/></w:r>
</w:p>
"@
}

function New-TableXml {
    param(
        [string[]]$Headers,
        [object[]]$Rows
    )

    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.Append("<w:tbl>")
    [void]$sb.Append("<w:tblPr><w:tblW w:w=`"0`" w:type=`"auto`"/><w:tblBorders><w:top w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"auto`"/><w:left w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"auto`"/><w:bottom w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"auto`"/><w:right w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"auto`"/><w:insideH w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"auto`"/><w:insideV w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"auto`"/></w:tblBorders></w:tblPr>")
    [void]$sb.Append("<w:tr>")
    foreach ($header in $Headers) {
        $h = Escape-Xml $header
        [void]$sb.Append("<w:tc><w:p><w:pPr><w:pStyle w:val=`"TableHeader`"/></w:pPr><w:r><w:t>$h</w:t></w:r></w:p></w:tc>")
    }
    [void]$sb.Append("</w:tr>")

    foreach ($row in $Rows) {
        [void]$sb.Append("<w:tr>")
        foreach ($cell in $row) {
            $value = Escape-Xml ([string]$cell)
            $value = $value -replace "`r`n|`n|`r", "</w:t><w:br/><w:t xml:space=`"preserve`">"
            [void]$sb.Append("<w:tc><w:p><w:r><w:t xml:space=`"preserve`">$value</w:t></w:r></w:p></w:tc>")
        }
        [void]$sb.Append("</w:tr>")
    }

    [void]$sb.Append("</w:tbl>")
    return $sb.ToString()
}

function Add-ZipEntry {
    param(
        [System.IO.Compression.ZipArchive]$Archive,
        [string]$EntryName,
        [string]$Content
    )

    $entry = $Archive.CreateEntry($EntryName)
    $stream = $entry.Open()
    $writer = New-Object System.IO.StreamWriter($stream, [System.Text.UTF8Encoding]::new($false))
    $writer.Write($Content)
    $writer.Dispose()
}

function Get-RouteUrl {
    param($controllerName, $baseRoute, $methodRoute)
    $base = $baseRoute.Replace("[controller]", $controllerName.Replace("Controller", ""))
    if ([string]::IsNullOrWhiteSpace($methodRoute)) { return "/$base" }
    return "/$base/$methodRoute"
}

function Get-StatusSummary {
    param([string]$Preview)
    $statuses = [System.Collections.Generic.List[string]]::new()
    if ($Preview -match "Ok\(") { $statuses.Add("200 OK") }
    if ($Preview -match "CreatedAtAction|Created\(") { $statuses.Add("201 Created") }
    if ($Preview -match "BadRequest\(") { $statuses.Add("400 Bad Request") }
    if ($Preview -match "Unauthorized\(") { $statuses.Add("401 Unauthorized") }
    if ($Preview -match "Forbid|403") { $statuses.Add("403 Forbidden") }
    if ($Preview -match "NotFound\(") { $statuses.Add("404 Not Found") }
    if ($statuses.Count -eq 0) { $statuses.Add("200/400/500 depending on flow") }
    return ($statuses | Select-Object -Unique) -join ", "
}

function Get-ValidationSummary {
    param([string]$Preview)
    $checks = [System.Collections.Generic.List[string]]::new()
    if ($Preview -match "ModelState\.IsValid") { $checks.Add("ModelState validation") }
    if ($Preview -match "string\.IsNullOrWhiteSpace") { $checks.Add("Required text/keyword check") }
    if ($Preview -match "NotFound") { $checks.Add("Existence check before update/delete/read") }
    if ($Preview -match "id != ") { $checks.Add("Route/body ID consistency check") }
    if ($Preview -match "Length > MaxImageBytes") { $checks.Add("File size limit") }
    if ($Preview -match "AllowedImageExtensions") { $checks.Add("File extension allow-list") }
    if ($checks.Count -eq 0) { $checks.Add("Minimal or delegated to repository/service") }
    return ($checks | Select-Object -Unique) -join "; "
}

function Get-BusinessSummary {
    param($method)
    $calls = @($method.Calls)
    if ($calls.Count -gt 0) {
        return "Delegates to: " + (($calls | Select-Object -Unique) -join ", ")
    }
    switch -Regex ($method.Name) {
        "Login" { return "Authenticates user credentials and issues JWT token." }
        "Get" { return "Reads data set or detail record." }
        "Create|Add|Open|Checkout|Approve|Receive|Fulfill|Save" { return "Creates or changes domain state in the database." }
        "Update|Change" { return "Updates existing record or status." }
        "Delete|Remove|Void" { return "Deletes or logically reverses data." }
        default { return "Controller orchestration endpoint." }
    }
}

function Get-AuthSummary {
    param($controller, $method)
    $auth = @()
    if (-not [string]::IsNullOrWhiteSpace($controller.ClassAuth)) { $auth += $controller.ClassAuth }
    if (-not [string]::IsNullOrWhiteSpace($method.Auth)) { $auth += $method.Auth }
    $auth = $auth | Where-Object { $_ } | Select-Object -Unique
    if ($auth.Count -eq 0) { return "No" }
    return "Yes - " + ($auth -join "; ")
}

$controllerInventory = Get-Content (Join-Path $projectRoot "controller_inventory.json") -Raw | ConvertFrom-Json
$tableInventory = Get-Content (Join-Path $projectRoot "table_inventory.json") -Raw | ConvertFrom-Json
$sourceFiles = Get-ChildItem $projectRoot -Recurse -File -Force | Where-Object {
    $_.FullName -notmatch "\\(bin|obj|\.vs)\\"
}
$sourceFileList = $sourceFiles | ForEach-Object {
    [pscustomobject]@{
        Path = $_.FullName.Replace($projectRoot + "\", "")
        Extension = $_.Extension
    }
} | Sort-Object Path

$topFolders = @(
    [pscustomobject]@{ Folder = ".github"; Responsibility = "CI/CD and automation workflow definitions." },
    [pscustomobject]@{ Folder = ".claude"; Responsibility = "Assistant tooling/editor metadata." },
    [pscustomobject]@{ Folder = "bingGooAPI"; Responsibility = "Main ASP.NET Core 8 Web API project containing controllers, repositories, services, queries, entities, models, middleware, configuration, and static file handling." },
    [pscustomobject]@{ Folder = "UnitTest"; Responsibility = "xUnit/Moq test project covering auth, brand, cart, category, currency, database integration, order, outlet, payment, product, and supplier flows." },
    [pscustomobject]@{ Folder = "src"; Responsibility = "Additional clean-architecture scaffold created during this workspace session; not yet integrated with the legacy API." },
    [pscustomobject]@{ Folder = "bin/obj/.vs"; Responsibility = "Generated build, IDE, and publish artifacts that should normally be excluded from version control." }
)

$folderResponsibilityRows = @(
    @("Controllers", "HTTP endpoints. Translate requests into repository/service calls and format IActionResult responses."),
    @("Repositories", "Primary data access layer using Dapper and SQL query constants."),
    @("Interfaces", "Contracts for controllers/services/repositories. Also contains one misplaced implementation (`AuthService.cs`)."),
    @("Services", "Domain/application helpers for auth, JWT, audit logging, outlet product logic, exchange rate, category, currency, and permissions."),
    @("DTOs / Models", "Request/response models used by API endpoints. Organized per module such as Product, Order, Outlet, User, Payment, and Reporting."),
    @("Entities", "Database/domain entities used by Dapper mapping."),
    @("Queries", "Raw SQL command store. Defines SELECT/INSERT/UPDATE/DELETE and stored procedure usage."),
    @("Helpers", "Support classes such as app settings wrappers, encryption helper, and placeholder `DataContext`."),
    @("Middlewares", "Cross-cutting request pipeline behavior, mainly centralized exception handling."),
    @("Configurations", "Dependency injection and JWT/database registration."),
    @("Attributes", "Custom authorization attribute bridging role-permission checks."),
    @("Properties", "Launch settings and publish profiles."),
    @("wwwroot", "Public static files, especially uploaded product images."),
    @("UnitTest", "Automated test project with mocked and integration-style tests."),
    @("appsettings*.json", "Runtime configuration including connection strings, JWT settings, logging, and CORS."),
    @("Solution files", "Entry points for Visual Studio solution composition.")
)

$packageRows = @(
    @("Dapper 2.1.35", "Primary micro-ORM for database access."),
    @("Microsoft.AspNetCore.Authentication.JwtBearer 8.0.1", "JWT bearer authentication."),
    @("Microsoft.EntityFrameworkCore.SqlServer 8.0.11", "Referenced but not meaningfully used in the current codebase."),
    @("Swashbuckle.AspNetCore 6.6.2", "Swagger/OpenAPI generation."),
    @("xUnit / Moq / Microsoft.NET.Test.Sdk / coverlet.collector", "Testing stack in `UnitTest`.")
)

$tableRows = @()
foreach ($tbl in $tableInventory) {
    if (@("DBJuJuBi", "source", "through") -contains $tbl.Table) { continue }
    $purpose = switch -Regex ($tbl.Table) {
        "^Users$" { "Application users, login identity, role assignment, outlet mapping, password hash, access flags." }
        "^Roles$" { "Role master used for RBAC and JWT role claim." }
        "^RolePermissions$" { "Join table between roles and permissions. Inferred from `PermissionRepository`." }
        "^Permissions$" { "Permission master used by `PermissionAuthorize`. Inferred from permission services." }
        "^TPRProducts$" { "Core product master catalog." }
        "^TblProductsScale$" { "Per-product packaging, size, weight, and UOM metadata." }
        "^Suppliers$" { "Supplier master." }
        "^Customer$" { "Customer master and loyalty points balance." }
        "^Orders$" { "Sales header / order header for checkout flow." }
        "^OrderItems$" { "Sales line items." }
        "^Payments$" { "Payment records for cash and KHQR workflows." }
        "^Carts$" { "Pre-checkout cart header." }
        "^CartItems$" { "Pre-checkout cart lines." }
        "^Outlet$" { "Outlet/branch store master." }
        "^OutletStock$" { "Inventory by outlet and product barcode." }
        "^OutletOrders$" { "Outlet replenishment order header." }
        "^OutletOrderItems$" { "Outlet replenishment order lines." }
        "^PurchaseOrders$" { "Procurement header from suppliers." }
        "^PurchaseOrderItems$" { "Procurement lines." }
        "^TransferOrders$" { "Stock transfer header between outlets." }
        "^TransferOrderItems$" { "Stock transfer lines." }
        "^MenuItem$" { "Sellable outlet-specific menu/pricing records." }
        "^Shifts$" { "POS cashier shift open/close records." }
        "^AuditLogs$" { "Audit trail for login and user actions." }
        "^VatSetting$" { "Single-row VAT percentage setting." }
        "^BankSetup$" { "Bank master for QR/bank setup." }
        "^Currency$" { "Currency master." }
        "^Category$" { "Category master." }
        "^UOM$" { "Unit-of-measure master." }
        "^Provinces$" { "Province lookup." }
        "^ProductDeliveryLogistics$" { "Province-based product delivery fees/logistics metadata." }
        "^ShelfLife$" { "Shelf life master/value." }
        "^tblTermDay$" { "Supplier payment term days." }
        "^Franchise$" { "Franchise master." }
        "^franchise_type$" { "Franchise type master." }
        "^OutletCode$" { "Outlet code registry." }
        "^OutletPhoto$" { "Outlet image collection." }
        "^CitizenshipPhotos$" { "Outlet legal identity/citizenship images." }
        "^PointTransactions$" { "Customer loyalty point ledger." }
        default { "Referenced by SQL query inventory." }
    }

    $tableRows += @(
        $tbl.Table,
        "Inferred from SQL usage",
        "Inferred from SQL joins/usage",
        (($tbl.QueryFiles -join ", ")),
        $purpose
    )
}

$endpointTables = [System.Collections.Generic.List[string]]::new()
foreach ($controller in $controllerInventory) {
    $endpointTables.Add((New-HeadingXml -Level 2 -Text $controller.Controller))
    $endpointTables.Add((New-ParagraphXml -Text "Base route: $($controller.BaseRoute)"))
    $rows = @()
    foreach ($method in $controller.Methods) {
        foreach ($http in $method.Http) {
            $rows += ,@(
                $http.Method.Replace("Http", "").ToUpperInvariant(),
                (Get-RouteUrl -controllerName $controller.Controller -baseRoute $controller.BaseRoute -methodRoute $http.Route),
                ($method.Parameters -replace "\s+", " ").Trim(),
                (Get-AuthSummary -controller $controller -method $method),
                (Get-BusinessSummary -method $method),
                (Get-ValidationSummary -Preview $method.BodyPreview),
                (Get-StatusSummary -Preview $method.BodyPreview)
            )
        }
    }
    $endpointTables.Add((New-TableXml -Headers @("Method", "URL", "Input", "Authentication", "Business Logic", "Validation", "Possible Errors / Status") -Rows $rows))
}

$fileInventoryRows = @()
foreach ($file in $sourceFileList) {
    $fileInventoryRows += ,@($file.Path, $file.Extension)
}

$fullFileCount = (Get-ChildItem $projectRoot -Recurse -File -Force | Measure-Object).Count
$extensionSummary = Get-ChildItem $projectRoot -Recurse -File -Force | Group-Object Extension | Sort-Object Count -Descending | Select-Object -First 15
$extensionText = ($extensionSummary | ForEach-Object { "{0}: {1}" -f (if ([string]::IsNullOrWhiteSpace($_.Name)) { "[no extension]" } else { $_.Name }), $_.Count }) -join "; "

$md = [System.Text.StringBuilder]::new()
[void]$md.AppendLine("# Project Documentation")
[void]$md.AppendLine()
[void]$md.AppendLine("Generated: July 24, 2026")
[void]$md.AppendLine()
[void]$md.AppendLine("Project analyzed recursively from `E:\\Setec\\MVC\\bingGooAPI`.")
[void]$md.AppendLine()
[void]$md.AppendLine("Total files scanned: $fullFileCount")
[void]$md.AppendLine()
[void]$md.AppendLine("Top extensions: $extensionText")
[void]$md.AppendLine()
[void]$md.AppendLine("This markdown file accompanies the Word document.")
Set-Content -Path $outputMd -Value $md.ToString()

$bodyParts = [System.Collections.Generic.List[string]]::new()
$bodyParts.Add((New-ParagraphXml -Text "Project Documentation" -Style "Title" -Justification "center"))
$bodyParts.Add((New-ParagraphXml -Text "Comprehensive Technical Review for the JuJuBi / bingGooAPI Solution" -Style "Subtitle" -Justification "center"))
$bodyParts.Add((New-ParagraphXml -Text "Generated on July 24, 2026" -Style "Subtitle" -Justification "center"))
$bodyParts.Add((New-ParagraphXml -Text "Prepared by Codex - Senior Software Architect, Technical Writer, and .NET Solution Reviewer" -Style "Subtitle" -Justification "center"))
$bodyParts.Add((New-PageBreakXml))
$bodyParts.Add((New-HeadingXml -Level 1 -Text "Table of Contents"))
$bodyParts.Add((New-TocXml))
$bodyParts.Add((New-PageBreakXml))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "1. Project Overview"))
$bodyParts.Add((New-TableXml -Headers @("Item", "Details") -Rows @(
    @("Project Name", "Primary project: JuJuBiAPI (`bingGooAPI\\JuJuBiAPI.csproj`). Solution names present: `bingGooAPI.sln` and `JuJuBiAPI.sln`."),
    @("Purpose", "Back-office and POS-oriented Web API covering authentication, users, products, suppliers, outlets, orders, payments, reports, stock transfer, purchase orders, and audit logging."),
    @("Business Goal", "Enable centralized retail/restaurant operations: product master management, outlet sales, warehouse replenishment, supplier procurement, cashier shifts, and reporting."),
    @("Technologies", "ASP.NET Core 8 Web API, SQL Server, Dapper, Swagger/OpenAPI, JWT Bearer Authentication, xUnit, Moq."),
    @("Framework", ".NET 8 (`net8.0`)."),
    @("Programming Language", "C#."),
    @("Database", "SQL Server (`DefaultConnection` targeting `DBJuJuBi`)."),
    @("Architecture", "Layered monolith with Controllers -> Services/Repositories -> Dapper SQL queries -> SQL Server."),
    @("Design Pattern", "Repository pattern, service abstraction, custom permission filter, middleware pipeline, DTO-based API boundaries."),
    @("State of Architecture", "Functional but not Clean Architecture. Significant business logic lives inside repositories and controllers, and SQL query constants are tightly coupled to modules.")
)))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "2. Folder Structure"))
$bodyParts.Add((New-ParagraphXml -Text "The solution was scanned recursively, including source, test, configuration, static assets, publish artifacts, and build outputs. The main source-bearing directories are listed below."))
$bodyParts.Add((New-TableXml -Headers @("Folder", "Responsibility") -Rows ($topFolders | ForEach-Object { ,@($_.Folder, $_.Responsibility) })))
$bodyParts.Add((New-TableXml -Headers @("Internal Folder", "Responsibility") -Rows $folderResponsibilityRows))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "3. Architecture"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Presentation Layer"))
$bodyParts.Add((New-ParagraphXml -Text "The presentation layer is the ASP.NET Core API in `bingGooAPI\\Controllers`. Controllers use attribute routing, return `IActionResult`, and rely on dependency injection for repository/service access. Authentication is mostly enforced with `[Authorize]`, with fine-grained permission checks layered through `PermissionAuthorizeAttribute`."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Business Layer"))
$bodyParts.Add((New-ParagraphXml -Text "Business logic is split between controllers, services, and repositories. Simple master data modules use thin controllers and direct repository calls. More complex workflows such as POS checkout, purchase receiving, transfer approval/receipt, and user password changes include service or repository-level orchestration."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Data Layer"))
$bodyParts.Add((New-ParagraphXml -Text "The data layer is Dapper-based. Repositories execute SQL constants stored under `bingGooAPI\\Queries`. Dapper maps directly to entities and DTOs. Query composition is not abstracted; SQL is module-local and explicit."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Infrastructure"))
$bodyParts.Add((New-ParagraphXml -Text "Infrastructure concerns are embedded in the same project: JWT generation, exception middleware, SQL connection registration, audit logging, file uploads to `wwwroot`, and Swagger registration."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Dependency Injection"))
$bodyParts.Add((New-ParagraphXml -Text "Dependencies are wired in `bingGooAPI\\Configurations\\ServicesConfigurations.cs`. Interfaces are registered to repositories or services with `AddScoped`, and an `IDbConnection` backed by `SqlConnection` is also registered as scoped."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Request Flow"))
$bodyParts.Add((New-CodeXml -Text "Client -> ASP.NET Core Middleware -> JWT Authentication -> Authorization / PermissionAuthorize -> Controller -> Service or Repository -> Dapper -> SQL Query Constant -> SQL Server"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Response Flow"))
$bodyParts.Add((New-CodeXml -Text "SQL Server -> Dapper mapping -> Repository / Service -> Controller IActionResult -> JSON response -> Client"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Repository Pattern"))
$bodyParts.Add((New-ParagraphXml -Text "Each domain area exposes an interface under `Interfaces` and an implementation under `Repositories` or `Services`. Repositories encapsulate SQL access but often contain transaction logic and significant business rules, so the repository pattern is present but overloaded."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Clean Architecture Assessment"))
$bodyParts.Add((New-ParagraphXml -Text "The project is not Clean Architecture. There is no separate application core, infrastructure project, or dependency rule separation. Controllers, repositories, services, entities, models, and SQL definitions all live in one web project."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Dapper Flow"))
$bodyParts.Add((New-CodeXml -Text "Controller -> Repository -> `QueryAsync` / `ExecuteAsync` / `QueryMultipleAsync` -> SQL constant in `Queries` -> SQL Server -> mapped entity/DTO"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "JWT Flow"))
$bodyParts.Add((New-CodeXml -Text "AuthController -> AuthService -> UserRepository -> PasswordHasher.VerifyPassword -> JwtTokenService.GenerateToken -> JWT claims (NameIdentifier, Name, OutletId, Role) -> client stores bearer token"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Authorization Flow"))
$bodyParts.Add((New-ParagraphXml -Text "First-line authorization uses `[Authorize]`. Role-specific restrictions use `[Authorize(Roles = \"ADMIN\")]`. Functional restrictions use `[PermissionAuthorize(\"CODE\")]`, which checks `RolePermissions` through `IPermissionRepository`."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Authentication Flow"))
$bodyParts.Add((New-ParagraphXml -Text "Authentication is username/password or MD password login. Password hashes are verified using PBKDF2 in `Models\\PasswordHasher.cs`. Successful login updates last login and generates a signed JWT."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Exception Flow"))
$bodyParts.Add((New-ParagraphXml -Text "Unhandled exceptions are trapped by `ExceptionMiddleware`. It maps SQL foreign key conflicts to friendly `400 Bad Request`, unauthorized access to `401`, missing keys to `404`, and all other errors to `500`."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Logging Flow"))
$bodyParts.Add((New-ParagraphXml -Text "Logging is partial. Global exception logging uses `ILogger<ExceptionMiddleware>`. Some business events are persisted through `AuditLogRepository` and `AuditLogger`. Request/response logging, correlation IDs, and structured telemetry are not implemented."))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "4. Database"))
$bodyParts.Add((New-ParagraphXml -Text "The solution does not include a full DDL schema file for the legacy API, so the database model below is inferred from all query files, joins, entity mappings, and transaction code."))
$bodyParts.Add((New-TableXml -Headers @("Table", "Primary Key", "Foreign Keys / Relationship", "Referenced By", "Purpose") -Rows $tableRows))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "ER Diagram Description"))
$bodyParts.Add((New-CodeXml -Text @"
Roles ----< Users >---- Outlet
  |            |
  |            +---- Orders ----< OrderItems >---- TPRProducts
  |                     |
  |                     +---- Payments
  |                     +---- Shifts
  |
  +---- RolePermissions >---- Permissions

Suppliers ----< PurchaseOrders ----< PurchaseOrderItems >---- TPRProducts

Outlet ----< OutletOrders ----< OutletOrderItems >---- TPRProducts
Outlet ----< TransferOrders ----< TransferOrderItems >---- TPRProducts
Outlet ----< OutletStock >---- TPRProducts

Customer ----< PointTransactions

Outlet ----< OutletPhoto
Outlet ----< CitizenshipPhotos
Outlet ---- Franchise ---- Franchise_Type
Outlet ---- Provinces
"@))
$bodyParts.Add((New-ParagraphXml -Text "Observed relationship patterns include one-to-many between Roles and Users, Outlet and Users, Outlet and orders, Orders and Payments, PurchaseOrders and PurchaseOrderItems, TransferOrders and TransferOrderItems, OutletOrders and OutletOrderItems, and logical joins between RolePermissions and Permissions."))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "5. API Documentation"))
$bodyParts.Add((New-ParagraphXml -Text "The following controller inventory was extracted from every controller file under `bingGooAPI\\Controllers`. Authentication and permission details are taken from attributes on each controller or action."))
foreach ($xml in $endpointTables) { $bodyParts.Add($xml) }

$bodyParts.Add((New-HeadingXml -Level 1 -Text "6. Code Flow"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "User Login"))
$bodyParts.Add((New-CodeXml -Text @"
User -> POST /api/auth/login
    -> AuthController.Login
    -> AuthService.LoginAsync
    -> UserRepository.GetByUsernameAsync
    -> PasswordHasher.VerifyPassword
    -> UserRepository.UpdateLastLoginAsync
    -> JwtTokenService.GenerateToken
    -> AuditLogger.LogAsync
    -> 200 OK { access_token, user }
"@))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "JWT Generation"))
$bodyParts.Add((New-ParagraphXml -Text "JWT generation occurs in `Services\\JwtTokenService.cs`. Claims include `NameIdentifier`, `Name`, `OutletId`, and `Role`. Signing uses symmetric HMAC SHA256 with configuration-driven key, issuer, audience, and expiration."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "CRUD Flow"))
$bodyParts.Add((New-CodeXml -Text @"
Create:
Client -> Controller -> Validate request -> Repository CreateAsync -> SQL INSERT -> return 201/200

Read:
Client -> Controller -> Repository GetAll/GetById/Search -> SQL SELECT -> mapped DTO/entity -> 200/404

Update:
Client -> Controller -> ID/existence checks -> Repository UpdateAsync -> SQL UPDATE -> 200/400/404

Delete:
Client -> Controller -> Existence / FK guard -> Repository DeleteAsync -> SQL DELETE or soft delete -> 200/400/404
"@))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "POS Checkout / Insert Flow"))
$bodyParts.Add((New-CodeXml -Text @"
POS terminal
 -> OrderController.PosCheckout
 -> OrderRepository.PosCheckoutAsync
 -> validate outlet stock per line
 -> validate customer loyalty points
 -> read VAT setting
 -> insert cart header
 -> insert/mirror cart items
 -> execute CheckoutCart stored procedure
 -> mark order paid
 -> apply points ledger adjustments
 -> deduct outlet stock
 -> return invoice/order payload
"@))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Update Flow"))
$bodyParts.Add((New-ParagraphXml -Text "Update flows usually perform route/body consistency checks, existence checks, then repository-level SQL updates. Complex update flows include transaction scopes for outlet, product scale, purchase receiving, outlet orders, and transfer receiving."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Delete Flow"))
$bodyParts.Add((New-ParagraphXml -Text "Delete flows are a mix of hard deletes and logical deactivation. Roles appear to be soft-deleted (`IsActive = 0`) while many other modules issue hard `DELETE` statements."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Search, Pagination, Filtering, Sorting"))
$bodyParts.Add((New-ParagraphXml -Text "Search is implemented for products, product scales, customers, and some reports. Filtering exists for reports and audit logs. Sorting is usually embedded in SQL `ORDER BY`. Pagination is mostly absent; large datasets are often returned in full."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Transaction Flow"))
$bodyParts.Add((New-ParagraphXml -Text "Transactions are used in repositories such as `ProductRepository`, `OutletRepository`, `PurchaseOrderRepository`, `TransferOrderRepository`, and `OutletOrderRepository`. However, `OrderRepository.PosCheckoutAsync` performs a multi-step critical financial flow without an explicit outer transaction, which is a major consistency risk."))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "7. Dependency Analysis"))
$bodyParts.Add((New-ParagraphXml -Text "Dependency registration is centralized in `Configurations\\ServicesConfigurations.cs`. The system relies heavily on constructor injection and module-specific repository interfaces."))
$bodyParts.Add((New-TableXml -Headers @("Package / Dependency", "Role in Solution") -Rows $packageRows))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Repository and Service Dependencies"))
$bodyParts.Add((New-ParagraphXml -Text "Controllers generally depend on a single repository or service per module, which keeps the surface predictable. Complex modules such as Auth, Orders, and Permissions depend on multiple collaborators. The design is simple to understand but can become tightly coupled because the web project owns every layer."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "External Integrations"))
$bodyParts.Add((New-ParagraphXml -Text "No major third-party APIs were found in source code beyond JWT/Swagger/SQL client libraries. Payment endpoints include KHQR request models, but direct external provider SDK integration is not visible in the scanned source files."))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "8. Security Review"))
$bodyParts.Add((New-TableXml -Headers @("Area", "Assessment", "Recommendation") -Rows @(
    @("JWT", "Implemented with issuer/audience/key validation. No refresh token, revocation, jti, or token store.", "Add refresh token flow, jti, revocation strategy, and secure key rotation."),
    @("Password Hashing", "Strong point. PBKDF2 with per-password salt and 100,000 iterations is implemented.", "Keep PBKDF2 or migrate to ASP.NET Identity/Argon2 for richer security lifecycle management."),
    @("SQL Injection", "Mostly mitigated because Dapper is used with parameterized queries.", "Continue parameterization and review any future dynamic SQL carefully."),
    @("XSS", "API-only surface lowers risk, but uploaded content and echoed strings still require frontend encoding discipline.", "Validate MIME signatures for uploads and ensure frontend output encoding."),
    @("CORS", "Falls back to `AllowAnyOrigin` if config is missing.", "Fail closed in non-development environments."),
    @("Authentication", "JWT bearer auth is applied broadly. Anonymous `HealthController` exposes DB connectivity check.", "Protect health endpoint or split internal readiness/liveness endpoints."),
    @("Authorization", "Combination of role and permission checks is good, but enforcement is inconsistent across some operational endpoints.", "Standardize permission mapping across all sensitive endpoints."),
    @("Rate Limiting", "Not implemented.", "Add ASP.NET Core rate limiting for auth, upload, and checkout endpoints."),
    @("Input Validation", "Mostly manual and inconsistent across controllers.", "Adopt FluentValidation or a centralized validation approach."),
    @("Secrets Management", "Critical issue: `appsettings.json` contains a live-looking SQL username/password connection string.", "Move secrets to environment variables, user secrets, or secure vault immediately.")
)))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "9. Performance Review"))
$bodyParts.Add((New-TableXml -Headers @("Finding", "Evidence", "Impact", "Recommendation") -Rows @(
    @("Very large SQL and repository files", "Examples: `Queries\\ProductQueries.cs` (439 lines), `Repositories\\ProductRepository.cs` (312 lines), `Repositories\\OrderRepository.cs` (311 lines).", "Harder maintenance, higher defect risk, and slower onboarding.", "Split modules by use case and extract shared SQL composition."),
    @("No pagination on many list endpoints", "Products, customers, suppliers, outlets, and others often return all rows.", "Risk of slow responses and memory pressure as data grows.", "Add page/filter/sort contracts consistently."),
    @("Potential N+1 reads in checkout", "`OrderRepository.PosCheckoutAsync` queries stock and product names per item.", "More round-trips under larger carts.", "Batch stock lookups per checkout."),
    @("Large transaction scripts in repositories", "Purchase order, outlet order, and transfer flows contain lengthy loops and repeated Dapper calls.", "Long transactions can hold locks and reduce concurrency.", "Push critical bulk logic into stored procedures or batched SQL commands."),
    @("Generated artifacts committed", "Repository includes extensive `bin`, `obj`, publish output, and uploaded files.", "Repository bloat and slower developer operations.", "Remove generated artifacts from source control and clean `.gitignore`."),
    @("Dead or low-value code", "`Helpers\\DataContext.cs` is empty. EF Core package is referenced but not substantively used.", "Noise and misleading architectural signals.", "Delete unused code and packages."),
    @("Shared scoped `IDbConnection` pattern", "Some repositories manually open/close the injected connection while others rely on implicit usage.", "Connection lifecycle inconsistency and transaction fragility.", "Use a connection factory or open-per-operation policy consistently.")
)))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "10. Best Practices Review"))
$bodyParts.Add((New-ParagraphXml -Text "Overall Score: 5.5 / 10"))
$bodyParts.Add((New-TableXml -Headers @("Dimension", "Score / 10", "Comment") -Rows @(
    @("Code Quality", "6", "Readable in many modules, but uneven naming, misplaced implementations, and oversized files reduce clarity."),
    @("Maintainability", "5", "Single-project monolith with mixed responsibilities and many hard-coded SQL strings."),
    @("Scalability", "5", "Operational workflows exist, but missing pagination, weak modular boundaries, and repo bloat will hurt growth."),
    @("Security", "5", "JWT and PBKDF2 are positives, but secret handling, public DB health endpoint, permissive CORS fallback, and missing rate limiting are concerns."),
    @("Performance", "5", "Dapper is efficient, but bulk/list design and multi-step flows need optimization."),
    @("Architecture", "5", "Layered structure exists, but not a clean separation of concerns or clean architecture.")
)))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Strengths"))
$bodyParts.Add((New-ParagraphXml -Text "Good use of Dapper for explicit SQL; broad API surface aligned with POS business modules; working JWT auth; custom permission filter for feature-level authorization; PBKDF2 password hashing; audit logging support; and transaction usage in several inventory/procurement flows."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Weaknesses"))
$bodyParts.Add((New-ParagraphXml -Text "Hard-coded credentials, inconsistent folder semantics, single-project monolith, large repositories/controllers, no rate limiting, public DB health endpoint, incomplete pagination, and committed build/publish artifacts."))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "11. Improvement Suggestions"))
$bodyParts.Add((New-TableXml -Headers @("Area", "Improvement") -Rows @(
    @("Folder Structure", "Move implementations out of `Interfaces`, split application/infrastructure/web projects, and separate module namespaces cleanly."),
    @("Architecture", "Adopt Clean Architecture or modular monolith boundaries: Web, Application, Domain, Infrastructure."),
    @("Database", "Add formal DDL/migration scripts, constraints documentation, indexes, and stored procedure inventory."),
    @("API", "Standardize result envelopes, validation, pagination, filtering, and error codes."),
    @("Naming", "Resolve inconsistent naming between `bingGooAPI`, `JuJuBiAPI`, `Brand` vs `Branch`, `ProductScal` vs `ProductScale`, `HouseOpration` spelling, and mixed DTO/entity names."),
    @("Error Handling", "Expand exception taxonomy and avoid raw generic exceptions for business rule failures."),
    @("Logging", "Add request logging, correlation IDs, structured logs, and security event logs."),
    @("Caching", "Introduce caching for lookup tables such as roles, permissions, provinces, currencies, VAT, and menu metadata."),
    @("Performance", "Batch SQL operations, add pagination, move heavy workflows into stored procedures or query objects, and add indexes on search fields."),
    @("Security", "Externalize secrets, lock down CORS, add rate limiting, secure health checks, add refresh tokens, and enhance upload validation.")
)))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "12. Complete Project Flow"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Generic Request Flow"))
$bodyParts.Add((New-CodeXml -Text @"
User
  ↓
Controller
  ↓
Service / Repository
  ↓
SQL Query Constant
  ↓
Dapper
  ↓
SQL Server
  ↓
Dapper Mapping
  ↓
Repository / Service
  ↓
Controller
  ↓
JSON Response
"@))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Product Creation"))
$bodyParts.Add((New-CodeXml -Text @"
User
  ↓
ProductController.Create
  ↓
ModelState validation
  ↓
ProductRepository.CreateAsync
  ↓
Duplicate barcode check
  ↓
Insert TPRProducts
  ↓
Insert TblProductsScale (optional)
  ↓
Commit transaction
  ↓
201 Created
"@))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Purchase Order Receive"))
$bodyParts.Add((New-CodeXml -Text @"
Warehouse user
  ↓
PurchaseOrderController.Receive
  ↓
PurchaseOrderRepository.ReceiveAsync
  ↓
Load purchase order and lines
  ↓
Validate remaining quantities
  ↓
Increment received quantity
  ↓
Upsert OutletStock
  ↓
Set status = Received / PartiallyReceived
  ↓
Commit transaction
  ↓
200 OK
"@))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Transfer Order Approval / Receipt"))
$bodyParts.Add((New-CodeXml -Text @"
Store / warehouse user
  ↓
TransferOrderController.Approve or Receive
  ↓
TransferOrderRepository
  ↓
Validate current status
  ↓
Approve: deduct source stock
  ↓
Receive: add destination stock and increment received qty
  ↓
Update transfer status
  ↓
Commit transaction
  ↓
200 OK
"@))

$bodyParts.Add((New-HeadingXml -Level 1 -Text "13. Generated Documentation Package"))
$bodyParts.Add((New-ParagraphXml -Text "This report was generated after recursively scanning the solution tree, including source code, tests, configuration files, static assets, and generated build/publish folders. The Word file is saved as `Project_Documentation.docx` in the project root because the workspace cannot safely write to the user Downloads directory from this environment."))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "File Inventory Appendix"))
$bodyParts.Add((New-ParagraphXml -Text "Non-generated file inventory scanned (excluding `bin`, `obj`, and `.vs` for readability in this appendix; generated folders were still counted in the overall scan totals)."))
$bodyParts.Add((New-TableXml -Headers @("Path", "Extension") -Rows $fileInventoryRows))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Generated Artifact Summary"))
$bodyParts.Add((New-ParagraphXml -Text "Total files scanned recursively: $fullFileCount"))
$bodyParts.Add((New-ParagraphXml -Text "Top file extensions: $extensionText"))
$bodyParts.Add((New-HeadingXml -Level 2 -Text "Summary"))
$bodyParts.Add((New-ParagraphXml -Text "The project is a substantial retail/POS back-office API with clear business intent and useful operational breadth. Its strongest technical assets are Dapper-based explicit SQL, PBKDF2 hashing, JWT auth, audit logging, and transactional handling in several inventory workflows. Its highest-priority risks are secret exposure, inconsistent authorization depth, public health connectivity exposure, repository/controller size, lack of pagination, and absence of a truly modular architecture."))

$documentBody = ($bodyParts -join "")

$documentXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:wpc="http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas"
 xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
 xmlns:o="urn:schemas-microsoft-com:office:office"
 xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
 xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math"
 xmlns:v="urn:schemas-microsoft-com:vml"
 xmlns:wp14="http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing"
 xmlns:wp="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"
 xmlns:w10="urn:schemas-microsoft-com:office:word"
 xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
 xmlns:w14="http://schemas.microsoft.com/office/word/2010/wordml"
 xmlns:wpg="http://schemas.microsoft.com/office/word/2010/wordprocessingGroup"
 xmlns:wpi="http://schemas.microsoft.com/office/word/2010/wordprocessingInk"
 xmlns:wne="http://schemas.microsoft.com/office/word/2006/wordml"
 xmlns:wps="http://schemas.microsoft.com/office/word/2010/wordprocessingShape"
 mc:Ignorable="w14 wp14">
  <w:body>
    $documentBody
    <w:sectPr>
      <w:footerReference w:type="default" r:id="rIdFooter1"/>
      <w:pgSz w:w="11906" w:h="16838"/>
      <w:pgMar w:top="1440" w:right="1440" w:bottom="1440" w:left="1440" w:header="720" w:footer="720" w:gutter="0"/>
    </w:sectPr>
  </w:body>
</w:document>
"@

$stylesXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:styles xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:docDefaults>
    <w:rPrDefault>
      <w:rPr>
        <w:rFonts w:ascii="Calibri" w:hAnsi="Calibri" w:eastAsia="Calibri" w:cs="Calibri"/>
        <w:sz w:val="22"/>
        <w:szCs w:val="22"/>
      </w:rPr>
    </w:rPrDefault>
  </w:docDefaults>
  <w:style w:type="paragraph" w:default="1" w:styleId="Normal">
    <w:name w:val="Normal"/>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Title">
    <w:name w:val="Title"/>
    <w:pPr><w:spacing w:before="200" w:after="200"/></w:pPr>
    <w:rPr><w:b/><w:sz w:val="36"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Subtitle">
    <w:name w:val="Subtitle"/>
    <w:pPr><w:spacing w:after="160"/></w:pPr>
    <w:rPr><w:i/><w:sz w:val="22"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading1">
    <w:name w:val="heading 1"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:uiPriority w:val="9"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:before="240" w:after="120"/></w:pPr>
    <w:rPr><w:b/><w:sz w:val="30"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading2">
    <w:name w:val="heading 2"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:uiPriority w:val="9"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:before="180" w:after="100"/></w:pPr>
    <w:rPr><w:b/><w:sz w:val="26"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading3">
    <w:name w:val="heading 3"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:uiPriority w:val="9"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:before="120" w:after="80"/></w:pPr>
    <w:rPr><w:b/><w:sz w:val="24"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Code">
    <w:name w:val="Code"/>
    <w:basedOn w:val="Normal"/>
    <w:pPr><w:spacing w:before="60" w:after="60"/><w:ind w:left="360"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/><w:sz w:val="18"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="TableHeader">
    <w:name w:val="TableHeader"/>
    <w:basedOn w:val="Normal"/>
    <w:rPr><w:b/></w:rPr>
  </w:style>
</w:styles>
"@

$settingsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:settings xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:updateFields w:val="true"/>
</w:settings>
"@

$footerXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:ftr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
 xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <w:p>
    <w:pPr><w:jc w:val="center"/></w:pPr>
    <w:r><w:t>Page </w:t></w:r>
    <w:r><w:fldChar w:fldCharType="begin"/></w:r>
    <w:r><w:instrText xml:space="preserve"> PAGE </w:instrText></w:r>
    <w:r><w:fldChar w:fldCharType="separate"/></w:r>
    <w:r><w:t>1</w:t></w:r>
    <w:r><w:fldChar w:fldCharType="end"/></w:r>
  </w:p>
</w:ftr>
"@

$docRelsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings" Target="settings.xml"/>
  <Relationship Id="rIdFooter1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer" Target="footer1.xml"/>
</Relationships>
"@

$rootRelsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties" Target="docProps/core.xml"/>
  <Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties" Target="docProps/app.xml"/>
</Relationships>
"@

$contentTypesXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/word/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"/>
  <Override PartName="/word/settings.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml"/>
  <Override PartName="/word/footer1.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml"/>
  <Override PartName="/docProps/core.xml" ContentType="application/vnd.openxmlformats-package.core-properties+xml"/>
  <Override PartName="/docProps/app.xml" ContentType="application/vnd.openxmlformats-officedocument.extended-properties+xml"/>
</Types>
"@

$coreXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<cp:coreProperties xmlns:cp="http://schemas.openxmlformats.org/package/2006/metadata/core-properties"
 xmlns:dc="http://purl.org/dc/elements/1.1/"
 xmlns:dcterms="http://purl.org/dc/terms/"
 xmlns:dcmitype="http://purl.org/dc/dcmitype/"
 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <dc:title>Project Documentation</dc:title>
  <dc:subject>Technical review of JuJuBiAPI / bingGooAPI</dc:subject>
  <dc:creator>Codex</dc:creator>
  <cp:keywords>ASP.NET Core; Dapper; SQL Server; JWT; POS</cp:keywords>
  <dc:description>Complete technical documentation generated from recursive solution analysis.</dc:description>
  <cp:lastModifiedBy>Codex</cp:lastModifiedBy>
  <dcterms:created xsi:type="dcterms:W3CDTF">2026-07-24T00:00:00Z</dcterms:created>
  <dcterms:modified xsi:type="dcterms:W3CDTF">2026-07-24T00:00:00Z</dcterms:modified>
</cp:coreProperties>
"@

$appXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"
 xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
  <Application>Codex</Application>
  <DocSecurity>0</DocSecurity>
  <ScaleCrop>false</ScaleCrop>
  <HeadingPairs>
    <vt:vector size="2" baseType="variant">
      <vt:variant><vt:lpstr>Title</vt:lpstr></vt:variant>
      <vt:variant><vt:i4>1</vt:i4></vt:variant>
    </vt:vector>
  </HeadingPairs>
  <TitlesOfParts>
    <vt:vector size="1" baseType="lpstr">
      <vt:lpstr>Project Documentation</vt:lpstr>
    </vt:vector>
  </TitlesOfParts>
  <Company>OpenAI</Company>
  <LinksUpToDate>false</LinksUpToDate>
  <SharedDoc>false</SharedDoc>
  <HyperlinksChanged>false</HyperlinksChanged>
  <AppVersion>1.0</AppVersion>
</Properties>
"@

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

if (Test-Path $outputDocx) {
    Remove-Item $outputDocx -Force
}

$archive = [System.IO.Compression.ZipFile]::Open($outputDocx, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    Add-ZipEntry -Archive $archive -EntryName "[Content_Types].xml" -Content $contentTypesXml
    Add-ZipEntry -Archive $archive -EntryName "_rels/.rels" -Content $rootRelsXml
    Add-ZipEntry -Archive $archive -EntryName "docProps/core.xml" -Content $coreXml
    Add-ZipEntry -Archive $archive -EntryName "docProps/app.xml" -Content $appXml
    Add-ZipEntry -Archive $archive -EntryName "word/document.xml" -Content $documentXml
    Add-ZipEntry -Archive $archive -EntryName "word/styles.xml" -Content $stylesXml
    Add-ZipEntry -Archive $archive -EntryName "word/settings.xml" -Content $settingsXml
    Add-ZipEntry -Archive $archive -EntryName "word/footer1.xml" -Content $footerXml
    Add-ZipEntry -Archive $archive -EntryName "word/_rels/document.xml.rels" -Content $docRelsXml
}
finally {
    $archive.Dispose()
}

Write-Output "Generated: $outputDocx"
Write-Output "Generated: $outputMd"
