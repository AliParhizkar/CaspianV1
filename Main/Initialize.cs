namespace Main
{
    public static class Initialize
    {
        public static void CreateFileAndFolder(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                var path = app.Environment.ContentRootPath + "\\Errors";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
    }
}
