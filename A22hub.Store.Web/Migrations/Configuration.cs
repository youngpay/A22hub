namespace A2208hub.Store.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    /*
     * �� NuGet ����̨���� Update-Database ��ͬ�����ݿ�䶯��Ŀǰ�õ����Զ�Ǩ��
     */
    /// <summary>
    /// ����Ǩ������
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<A2208hub.Store.Web.Models.StoreDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(A2208hub.Store.Web.Models.StoreDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
