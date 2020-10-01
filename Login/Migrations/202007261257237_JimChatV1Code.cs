namespace Login.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JimChatV1Code : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        User1 = c.String(),
                        User2 = c.String(),
                        Message = c.String(),
                        IsActivity = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        FirstCookie = c.String(),
                        RoomID = c.String(),
                        Choose = c.String(),
                        Wait = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Rooms");
        }
    }
}
