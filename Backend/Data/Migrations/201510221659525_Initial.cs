namespace AlarmWorkflow.Backend.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Represents the initial migration.
    /// </summary>
    public partial class Initial : DbMigration
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.dispresource",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        operation_id = c.Int(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        emkresourceid = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.operation", t => t.operation_id, cascadeDelete: true)
                .Index(t => t.operation_id);
            
            CreateTable(
                "dbo.operation",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        acknowledged = c.Boolean(nullable: false),
                        operationguid = c.Guid(nullable: false),
                        operationnumber = c.String(unicode: false),
                        timestampincome = c.DateTime(nullable: false, precision: 0),
                        timestampalarm = c.DateTime(nullable: false, precision: 0),
                        messenger = c.String(unicode: false),
                        comment = c.String(unicode: false),
                        plan = c.String(unicode: false),
                        picture = c.String(unicode: false),
                        priority = c.String(unicode: false),
                        einsatzortlocation = c.String(unicode: false),
                        einsatzortzipcode = c.String(unicode: false),
                        einsatzortcity = c.String(unicode: false),
                        einsatzortstreet = c.String(unicode: false),
                        einsatzortstreetnumber = c.String(unicode: false),
                        einsatzortintersection = c.String(unicode: false),
                        einsatzortproperty = c.String(unicode: false),
                        einsatzortlatlng = c.String(unicode: false),
                        zielortlocation = c.String(unicode: false),
                        zielortzipcode = c.String(unicode: false),
                        zielortcity = c.String(unicode: false),
                        zielortstreet = c.String(unicode: false),
                        zielortstreetnumber = c.String(unicode: false),
                        zielortintersection = c.String(unicode: false),
                        zielortproperty = c.String(unicode: false),
                        zielortlatlng = c.String(unicode: false),
                        keyword = c.String(unicode: false),
                        keywordmisc = c.String(unicode: false),
                        keywordb = c.String(unicode: false),
                        keywordr = c.String(unicode: false),
                        keywords = c.String(unicode: false),
                        keywordt = c.String(unicode: false),
                        loopscsv = c.String(unicode: false),
                        customdatajson = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.operationresource",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        operation_id = c.Int(nullable: false),
                        timestamp = c.String(unicode: false),
                        fullname = c.String(unicode: false),
                        equipmentcsv = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.operation", t => t.operation_id, cascadeDelete: true)
                .Index(t => t.operation_id);
            
            CreateTable(
                "dbo.usersetting",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        identifier = c.String(nullable: false, unicode: false),
                        name = c.String(nullable: false, unicode: false),
                        value = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.operationresource", "operation_id", "dbo.operation");
            DropForeignKey("dbo.dispresource", "operation_id", "dbo.operation");
            DropIndex("dbo.operationresource", new[] { "operation_id" });
            DropIndex("dbo.dispresource", new[] { "operation_id" });
            DropTable("dbo.usersetting");
            DropTable("dbo.operationresource");
            DropTable("dbo.operation");
            DropTable("dbo.dispresource");
        }
    }
}
