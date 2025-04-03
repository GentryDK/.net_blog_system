using AbpvNextDemo.EntityFrameworkCore;
using AbpvNextDemo.Web;
using Microsoft.Extensions.DependencyInjection;

//这里是为了在部署后的环境下可以迁移数据库的，我们在core层中写了个Factory可以使用控制台了，但是没法在
//部署环境下使用

//在部署环境中，通常不会有开发环境中的设计时工具（例如 Visual Studio）来帮助你执行数据库迁移。
//因此，你需要一种方法在应用程序启动时自动执行数据库迁移。通过在代码中显式调用 Migrator 方法，
//你可以确保在应用程序启动时执行数据库迁移，而不需要手动运行迁移命令
//这种方法避免了手动运行迁移命令的麻烦，并确保数据库结构始终与最新的模型保持一致。

//这里简单说就是在控制台中从容器中拿到UserSystemDbContext然后调用Migrator就行
IServiceCollection services = new ServiceCollection();
services.AddApplication<UserSystemWebModule>();
var serviceProvider = services.BuildServiceProvider();
var dbContext = serviceProvider.GetRequiredService<UserSystemDbContext>();

//这里调用 dbContext 的 Migrator 方法来执行数据库迁移。
//Migrator 方法的作用通常是应用数据库迁移，
//将数据库结构更新到最新的版本。
//这可能包括创建、修改或删除数据库表和列，以匹配最新的数据库模型
dbContext.Migrator();
