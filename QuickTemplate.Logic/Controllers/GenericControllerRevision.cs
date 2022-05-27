//@BaseCode
//MdStart
#if ACCOUNT_ON && REVISION_ON
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickTemplate.Logic.Controllers
{
    partial class GenericController<TEntity>
    {
        private enum RevisionType
        {
            Insert,
            Update,
            Delete,
        }
        private record HistoryItem(int IdentityId, RevisionType RevisionType, DateTime ActionTime, Entities.IdentityEntity Entity, string? JsonData);
        private readonly List<HistoryItem> historyItems = new();

        partial void AfterExecuteInsert(TEntity entity)
        {
            if (entity.GetType() != typeof(Entities.Revision.History))
            {
                var loginSession = Modules.Account.AccountManager.QueryLoginSession(SessionToken);

                if (loginSession != null)
                {
                    historyItems.Add(new HistoryItem(loginSession.IdentityId, RevisionType.Insert, DateTime.Now, entity, null));
                }
            }
        }
        partial void BeforeExecuteUpdate(TEntity entity)
        {
            var historyItem = historyItems.FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (historyItem == null)
            {
                var oriEntity = EntitySet.Where(e => e.Id == entity.Id).AsNoTracking().FirstOrDefault();

                if (oriEntity != null)
                {
                    var loginSession = Modules.Account.AccountManager.QueryLoginSession(SessionToken);

                    if (loginSession != null)
                    {
                        historyItems.Add(new HistoryItem(loginSession.IdentityId, RevisionType.Update, DateTime.Now, entity, JsonSerializer.Serialize(oriEntity, new JsonSerializerOptions() { MaxDepth = 0, })));
                    }
                }
            }
        }
        partial void BeforeExecuteDelete(TEntity entity)
        {
            var loginSession = Modules.Account.AccountManager.QueryLoginSession(SessionToken);

            if (loginSession != null)
            {
                historyItems.Add(new HistoryItem(loginSession.IdentityId, RevisionType.Delete, DateTime.Now, entity, JsonSerializer.Serialize(entity, new JsonSerializerOptions() { MaxDepth = 0 })));
            }
        }
        partial void AfterExecuteSaveChanges()
        {
            if (historyItems.Any())
            {
                Task.Run(async () =>
                {
                    using var ctrl = new Revision.HistoriesController();

                    ctrl.SessionToken = Modules.Security.Authorization.SystemAuthorizationToken;
                    foreach (var item in historyItems)
                    {
                        var history = new Entities.Revision.History()
                        {
                            IdentityId = item.IdentityId,
                            ActionType = item.RevisionType.ToString(),
                            ActionTime = item.ActionTime,
                            SubjectName = item.Entity.GetType().Name,
                            SubjectId = item.Entity.Id,
                            JsonData = item.JsonData ?? string.Empty,
                        };
                        _ = await ctrl.ExecuteInsertAsync(history).ConfigureAwait(false);
                    }
                    _ = await ctrl.SaveChangesAsync().ConfigureAwait(false); 
                }).Wait();
                historyItems.Clear();
            }
        }
    }
}
#endif
//MdEnd