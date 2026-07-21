namespace JuJuBiAPI.Queries
{
    public static class RoleQueries
    {
        public const string Create = @"
        INSERT INTO Roles
        (
            RoleCode,
            RoleName,
            Description,
            IsSystemRole,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            @RoleCode,
            @RoleName,
            @Description,
            @IsSystemRole,
            @IsActive,
            GETDATE()
        );
    ";

        public const string Delete = @"
  update Roles set IsActive = 0
  where Id = @id";

        public const string GetAll = @"SELECT [Id]
      ,[RoleCode]
      ,[RoleName]
      ,[Description]
      ,[IsSystemRole]
      ,[IsActive]
      ,[CreatedAt]
      ,[UpdatedAt]
  FROM [dbo].[Roles]
";

        public const string GetById = @"
        SELECT
            Id,
            RoleCode,
            RoleName,
            Description,
            IsSystemRole,
            IsActive
        FROM Roles
        WHERE Id = @id and IsActive = 1
    ";

        public const string Update = @"  update Roles set
  RoleCode = @RoleCode,
  RoleName = @RoleName,
  Description = @Description,
  IsSystemRole = @IsSystemRole,
  IsActive = @IsActive ,
  UpdatedAt = GETDATE()
  where Id = @id";

        public const string GetNextCode = "SELECT ISNULL(MAX(Id), 0) + 1 FROM Roles;";
    }
}
