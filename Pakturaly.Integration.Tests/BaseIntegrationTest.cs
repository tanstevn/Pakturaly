using Microsoft.EntityFrameworkCore;
using Pakturaly.Data;

namespace Pakturaly.Integration.Tests {
    public abstract class BaseIntegrationTest {
        protected ApplicationDbContext DbContext { get; private set; }

        protected BaseIntegrationTest() {
            var dbName = Guid.NewGuid()
                .ToString();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            DbContext = new ApplicationDbContext(contextOptions, null!);
        }
    }
}
