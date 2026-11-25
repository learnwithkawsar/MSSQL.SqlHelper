# MSSQL.SqlHelper

SqlHelper is a simple class to help with Microsoft SQL Server databases on **.NET Framework**.
Run stored procedures or T-SQL and get back a `DataSet`, `DataTable`, `SqlDataReader`,
`XmlReader`, a scalar value or the number of affected rows, using a connection string,
an open `SqlConnection` or a `SqlTransaction`.

Targets .NET Framework 4.6.2 and later, and uses
[`Microsoft.Data.SqlClient`](https://www.nuget.org/packages/Microsoft.Data.SqlClient).

> Using .NET Core or modern .NET (8, 10, ...)? Use
> [Core.MSSQL.SqlHelper](https://www.nuget.org/packages/Core.MSSQL.SqlHelper) instead. It adds
> async methods and ASP.NET Core dependency injection.

## Install

```powershell
Install-Package MSSQL.SqlHelper
```

## Use

```csharp
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using MSSQL.SqlHelper;

string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

// Stored procedure with parameters
DataTable users = SqlHelper.ExecuteDataTable(
    connectionString,
    CommandType.StoredProcedure,
    "dbo.GetUsers",
    new SqlParameter("@IsActive", true));

// Parameterized SQL (never concatenate user input into SQL)
int updated = SqlHelper.ExecuteNonQuery(
    connectionString,
    CommandType.Text,
    "UPDATE dbo.Users SET LastSeen = SYSUTCDATETIME() WHERE Id = @id",
    new SqlParameter("@id", 42));

// Scalar value
object count = SqlHelper.ExecuteScalar(connectionString, CommandType.Text, "SELECT COUNT(*) FROM dbo.Users");
```

### Command timeout

All commands use `SqlHelper.CommandTimeout` (in seconds). It defaults to `0` (no timeout)
for backward compatibility; setting a real value is recommended:

```csharp
SqlHelper.CommandTimeout = 60;
```

## Upgrading from 1.x

- **Requires .NET Framework 4.6.2 or later** (1.x targeted 4.5.1).
- **`System.Data.SqlClient` was replaced by `Microsoft.Data.SqlClient`.** Change
  `using System.Data.SqlClient;` to `using Microsoft.Data.SqlClient;` in your code.
  Note that `Microsoft.Data.SqlClient` encrypts connections by default (`Encrypt=True`); for a
  local SQL Server without a trusted certificate, add `TrustServerCertificate=True` to the
  connection string.
- **The command timeout is now configurable** through `SqlHelper.CommandTimeout`.

## Releasing (maintainers)

Pushing a version tag publishes the package to NuGet through
[`.github/workflows/publish.yml`](.github/workflows/publish.yml), using
[NuGet trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing),
so no API key is stored.

1. One-time setup:
   - On nuget.org, open your username menu → **Trusted Publishing** and add a policy with
     Repository Owner `learnwithkawsar`, Repository `MSSQL.SqlHelper`, Workflow File `publish.yml`,
     no Environment, and package `MSSQL.SqlHelper`.
   - In this GitHub repository, add the secret `NUGET_USER` with your nuget.org username
     (profile name, not email) under Settings → Secrets and variables → Actions.
2. Set `<Version>` in `MSSQL.SqlHelper/MSSQL.SqlHelper.csproj` and merge to `master`.
3. Tag that commit with the same version and push the tag:

   ```powershell
   git tag v2.0.0
   git push origin v2.0.0
   ```

The workflow fails without publishing if the tag doesn't match `<Version>` or `NUGET_USER` is missing.
It builds and tests before pushing, and skips a version that is already on NuGet.

## License

MIT
