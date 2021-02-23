using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.EntityClient;

namespace HisModel
{
    public partial class DLEntitiesFactory
    {
        public static DLEntities CreateEntitie()
        {
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Metadata = "res://*/HisModel.csdl|res://*/HisModel.ssdl|res://*/HisModel.msl";
            entityBuilder.ProviderConnectionString = @"data source=124.204.65.84,12502;initial catalog=HistoryNews$HealthMedia;persist security info=True;user id=sa;password=qazwsxEDC!@#;MultipleActiveResultSets=True;App=EntityFramework;" ;
            entityBuilder.Provider = "System.Data.SqlClient";
            return new DLEntities(entityBuilder.ToString());
        }
    }
}
