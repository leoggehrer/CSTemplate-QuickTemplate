//@BaseCode
//MdStart
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickTemplate.Logic.UnitTest
{
    /// <summary>
    /// This class provides basic functions for testing entities (insert, insert array, update, update array).
    /// </summary>
    /// <typeparam name="T">The generic parameter of the entity.</typeparam>
    [TestClass]
    public abstract class EntityUnitTest<T> where T : Entities.IdentityEntity, new()
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static int Counter = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public abstract Controllers.GenericController<T> CreateController();

        public List<string> IgnoreUpdateProperties = new() { nameof(Entities.IdentityEntity.Id), nameof(Entities.VersionEntity.RowVersion) };
        /// <summary>
        /// This method deletes all entities in the database.
        /// </summary>
        /// <returns></returns>
        public virtual async Task DeleteControllerEntities()
        {
            using var ctrl = CreateController();
            var items = await ctrl.GetAllAsync();

            foreach (var item in items)
            {
                await ctrl.DeleteAsync(item.Id);
            }
            await ctrl.SaveChangesAsync();
        }

        /// <summary>
        /// This method creates an entity in the database, reads this entity again and compares it with the input.
        /// </summary>
        /// <param name="entity">Entity created in the database.</param>
        /// <returns>The actual entity</returns>
        public virtual async Task<T> Create_Entity_AndCheck(T entity)
        {
            try
            {
                using var ctrl = CreateController();
                using var ctrlAfter = CreateController();

                var insertEntity = await ctrl.InsertAsync(entity);

                Assert.IsNotNull(insertEntity);
                await ctrl.SaveChangesAsync();

                var actualEntity = await ctrlAfter.GetByIdAsync(insertEntity.Id);

                Assert.IsNotNull(actualEntity);
                Assert.IsTrue(insertEntity.AreEqualProperties(actualEntity));
                return actualEntity;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

                throw;
            }
        }

        /// <summary>
        /// This method creates an array of entities in the database, re-reads those entities and compares them to the input.
        /// </summary>
        /// <param name="entities">Entities created in the database.</param>
        /// <returns></returns>
        public virtual async Task CreateArray_Entities_AndCheckAll(IEnumerable<T> entities)
        {
            using var ctrl = CreateController();
            using var ctrlAfter = CreateController();

            var insertEntities = await ctrl.InsertAsync(entities);

            Assert.IsNotNull(insertEntities);
            Assert.AreEqual(entities.Count(), insertEntities.Count());
            await ctrl.SaveChangesAsync();

            foreach (var item in insertEntities)
            {
                var actualItem = await ctrlAfter.GetByIdAsync(item.Id);

                Assert.IsNotNull(actualItem);
                Assert.IsTrue(item.AreEqualProperties(actualItem));
            }
        }

        /// <summary>
        /// This method updates an entity in the database, rereads that entity, modifies it, and saves the change. 
        /// The entity is then read out again and compared with the input.
        /// </summary>
        /// <param name="id">Id form entity updated in the Database.</param>
        /// <param name="changedEntity">Entity containing the changes.</param>
        /// <returns>The actual entity</returns>
        public virtual async Task<T> Update_Entity_AndCheck(int id, T changedEntity)
        {
            using var ctrl = CreateController();
            using var ctrlAfter = CreateController();
            using var ctrlUpdate = CreateController();
            using var ctrlUpdateAfter = CreateController();

            var actualEntity = await ctrlAfter.GetByIdAsync(id);

            Assert.IsNotNull(actualEntity);

            actualEntity.CopyFrom(changedEntity, n => IgnoreUpdateProperties.Contains(n) == false);

            var updateEntity = await ctrlUpdate.UpdateAsync(actualEntity);

            Assert.IsNotNull(updateEntity);
            await ctrlUpdate.SaveChangesAsync();

            var actualUpdateEntity = await ctrlUpdateAfter.GetByIdAsync(id);

            Assert.IsNotNull(actualUpdateEntity);
            Assert.IsTrue(changedEntity.AreEqualProperties(actualUpdateEntity, IgnoreUpdateProperties.ToArray()));
            return actualUpdateEntity;
        }

        /// <summary>
        /// This method creates an entity in the database, rereads that entity, modifies it, and saves the change. 
        /// The entity is then read out again and compared with the input.
        /// </summary>
        /// <param name="entity">Entity created in the Database.</param>
        /// <param name="changedEntity">Entity containing the changes.</param>
        /// <returns>The actual entity</returns>
        public virtual async Task<T> CreateUpdate_Entity_AndCheck(T entity, T changedEntity)
        {
            using var ctrl = CreateController();
            using var ctrlAfter = CreateController();
            using var ctrlUpdate = CreateController();
            using var ctrlUpdateAfter = CreateController();

            var insertEntity = await ctrl.InsertAsync(entity);

            Assert.IsNotNull(insertEntity);
            await ctrl.SaveChangesAsync();

            var actualEntity = await ctrlAfter.GetByIdAsync(insertEntity.Id);

            Assert.IsNotNull(actualEntity);
            Assert.IsTrue(insertEntity.AreEqualProperties(actualEntity));

            actualEntity.CopyFrom(changedEntity, n => IgnoreUpdateProperties.Contains(n) == false);

            var updateEntity = await ctrlUpdate.UpdateAsync(actualEntity);

            Assert.IsNotNull(updateEntity);
            await ctrlUpdate.SaveChangesAsync();

            var actualUpdateEntity = await ctrlUpdateAfter.GetByIdAsync(insertEntity.Id);

            Assert.IsNotNull(actualUpdateEntity);
            Assert.IsTrue(changedEntity.AreEqualProperties(actualUpdateEntity, IgnoreUpdateProperties.ToArray()));
            return actualUpdateEntity;
        }

        /// <summary>
        /// This method creates an array of entities in the database, re-reads that entity, modifies it, and saves the change. 
        /// The entities are then read out again and compared with the input.
        /// </summary>
        /// <param name="entities">Entities created in the database.</param>
        /// <param name="changedEntities">Entities containing the changes.</param>
        /// <returns></returns>
        public virtual async Task CreateUpdateArray_Entities_AndCheckAll(IEnumerable<T> entities, IEnumerable<T> changedEntities)
        {
            using var ctrl = CreateController();
            using var ctrlAfter = CreateController();
            using var ctrlUpdate = CreateController();
            using var ctrlUpdateAfter = CreateController();
            var actualEntities = new List<T>();

            Assert.AreEqual(entities.Count(), changedEntities.Count());

            var insertEntities = await ctrl.InsertAsync(entities);

            Assert.IsNotNull(insertEntities);
            await ctrl.SaveChangesAsync();

            foreach (var item in insertEntities)
            {
                var actualEntity = await ctrlAfter.GetByIdAsync(item.Id);

                Assert.IsNotNull(actualEntity);
                Assert.IsTrue(item.AreEqualProperties(actualEntity));
                actualEntities.Add(actualEntity);
            }

            var changeArray = changedEntities.ToArray();

            for (int i = 0; i < actualEntities.Count; i++)
            {
                var actualEntity = actualEntities[i];
                var changedEntity = changeArray[i];

                actualEntity.CopyFrom(changedEntity, n => IgnoreUpdateProperties.Contains(n) == false);
            }

            var updateEntities = await ctrlUpdate.UpdateAsync(actualEntities);

            Assert.IsNotNull(updateEntities);
            await ctrlUpdate.SaveChangesAsync();

            foreach (var item in updateEntities)
            {
                var actualUpdateEntity = await ctrlUpdateAfter.GetByIdAsync(item.Id);

                Assert.IsNotNull(actualUpdateEntity);
                Assert.IsTrue(item.AreEqualProperties(actualUpdateEntity));
            }
        }
    }
}
//MdEnd