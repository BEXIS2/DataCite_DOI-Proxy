using LiteDB;
using Newtonsoft.Json;
using NUnit.Framework.Legacy;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Types;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            using var db = new LiteDatabase("Filename=C:/Projects/github.com/sventhiel/Vaelastrasz/Vaelastrasz.Server/Database/Vaelastrasz.db;Connection=Shared");

            var userService = new UserService(new LiteDB.ConnectionString("Filename=C:/Projects/github.com/sventhiel/Vaelastrasz/Vaelastrasz.Server/Database/Vaelastrasz.db;Connection=Shared"));

            //    var users = db.GetCollection<User>("users");
            //var user = users.FindById(null);

            var user = await userService.GetByIdAsync(0);

            ClassicAssert.IsNull(user);
        }

        [Test]
        public async Task Test2()
        {
            var accountType = EnumExtensions.GetEnumMemberValue(AccountType.DataCite);
            var type = await Task.FromResult(JsonConvert.SerializeObject(DataCiteType.DOIs));
        }
    }
}