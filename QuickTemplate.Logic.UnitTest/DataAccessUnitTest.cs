//@BaseCode
//MdStart
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickTemplate.Logic.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickTemplate.Logic.UnitTest
{
    /// <summary>
    /// This class provides basic functions for testing entities (insert, insert array, update, update array).
    /// </summary>
    /// <typeparam name="TAccessModel">The generic parameter of the model.</typeparam>
    [TestClass]
    public abstract class DataAccessUnitTest<TAccessModel> where TAccessModel : IIdentifyable, new()
    {
        public static int Counter = 0;

        public abstract IDataAccess<TAccessModel> CreateDataAccess();

        public List<string> IgnoreUpdateProperties = new() { nameof(IIdentifyable.Id), nameof(IVersionable.RowVersion) };
        /// <summary>
        /// This method deletes all entities in the database.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteControllerEntities()
        {
            using var dataAccess = CreateDataAccess();
            var items = await dataAccess.GetAllAsync();

            foreach (var item in items)
            {
                await dataAccess.DeleteAsync(item.Id);
            }
            await dataAccess.SaveChangesAsync();
        }

        /// <summary>
        /// This method creates an model in the database, reads this model again and compares it with the input.
        /// </summary>
        /// <param name="accessModel">Model created in the database.</param>
        /// <returns>The actuel model</returns>
        public async Task<TAccessModel> Create_AccessModel_AndCheck(TAccessModel accessModel)
        {
            try
            {
                using var dataAccess = CreateDataAccess();
                using var ctrlAfter = CreateDataAccess();

                var insertModel = await dataAccess.InsertAsync(accessModel);

                Assert.IsNotNull(insertModel);
                await dataAccess.SaveChangesAsync();

                var actualModel = await ctrlAfter.GetByIdAsync(insertModel.Id);

                Assert.IsNotNull(actualModel);
                Assert.IsTrue(insertModel.AreEqualProperties(actualModel));
                return actualModel;
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
        /// <param name="accessModels">Models created in the database.</param>
        /// <returns></returns>
        public async Task CreateArray_AccessModels_AndCheckAll(IEnumerable<TAccessModel> accessModels)
        {
            using var dataAccess = CreateDataAccess();
            using var dataAccessAfter = CreateDataAccess();

            var insertModels = await dataAccess.InsertAsync(accessModels);

            Assert.IsNotNull(insertModels);
            Assert.AreEqual(accessModels.Count(), insertModels.Count());
            await dataAccess.SaveChangesAsync();

            foreach (var item in insertModels)
            {
                var actualItem = await dataAccessAfter.GetByIdAsync(item.Id);

                Assert.IsNotNull(actualItem);
                Assert.IsTrue(item.AreEqualProperties(actualItem));
            }
        }

        /// <summary>
        /// This method updates an model in the database, rereads that model, modifies it, and saves the change. 
        /// The model is then read out again and compared with the input.
        /// </summary>
        /// <param name="id">Id form model updated in the Database.</param>
        /// <param name="changedModel">Model containing the changes.</param>
        /// <returns>The actual model</returns>
        public async Task<TAccessModel> Update_AccessModel_AndCheck(int id, TAccessModel changedModel)
        {
            using var dataAccess = CreateDataAccess();
            using var dataAccessAfter = CreateDataAccess();
            using var dataAccessUpdate = CreateDataAccess();
            using var dataAccessUpdateAfter = CreateDataAccess();

            var actualModel = await dataAccessAfter.GetByIdAsync(id);

            Assert.IsNotNull(actualModel);

            actualModel.CopyFrom(changedModel, n => IgnoreUpdateProperties.Contains(n) == false);

            var updateModel = await dataAccessUpdate.UpdateAsync(actualModel);

            Assert.IsNotNull(updateModel);
            await dataAccessUpdate.SaveChangesAsync();

            var actualUpdateModel = await dataAccessUpdateAfter.GetByIdAsync(id);

            Assert.IsNotNull(actualUpdateModel);
            Assert.IsTrue(changedModel.AreEqualProperties(actualUpdateModel, IgnoreUpdateProperties.ToArray()));
            return actualUpdateModel;
        }

        /// <summary>
        /// This method creates an model in the database, rereads that model, modifies it, and saves the change. 
        /// The model is then read out again and compared with the input.
        /// </summary>
        /// <param name="accessModel">Model created in the Database.</param>
        /// <param name="changedModel">Model containing the changes.</param>
        /// <returns>The actual model</returns>
        public async Task<TAccessModel> CreateUpdate_AccessModel_AndCheck(TAccessModel accessModel, TAccessModel changedModel)
        {
            using var dataAccess = CreateDataAccess();
            using var dataAccessAfter = CreateDataAccess();
            using var dataAccessUpdate = CreateDataAccess();
            using var dataAccessUpdateAfter = CreateDataAccess();

            var insertModel = await dataAccess.InsertAsync(accessModel);

            Assert.IsNotNull(insertModel);
            await dataAccess.SaveChangesAsync();

            var actualModel = await dataAccessAfter.GetByIdAsync(insertModel.Id);

            Assert.IsNotNull(actualModel);
            Assert.IsTrue(insertModel.AreEqualProperties(actualModel));

            actualModel.CopyFrom(changedModel, n => IgnoreUpdateProperties.Contains(n) == false);

            var updateModel = await dataAccessUpdate.UpdateAsync(actualModel);

            Assert.IsNotNull(updateModel);
            await dataAccessUpdate.SaveChangesAsync();

            var actualUpdateModel = await dataAccessUpdateAfter.GetByIdAsync(insertModel.Id);

            Assert.IsNotNull(actualUpdateModel);
            Assert.IsTrue(changedModel.AreEqualProperties(actualUpdateModel, IgnoreUpdateProperties.ToArray()));
            return actualUpdateModel;
        }

        /// <summary>
        /// This method creates an array of models in the database, re-reads that model, modifies it, and saves the change. 
        /// The models are then read out again and compared with the input.
        /// </summary>
        /// <param name="accessModels">Models created in the database.</param>
        /// <param name="changedModels">Models containing the changes.</param>
        /// <returns></returns>
        public async Task CreateUpdateArray_AccessModels_AndCheckAll(IEnumerable<TAccessModel> accessModels, IEnumerable<TAccessModel> changedModels)
        {
            using var dataAccess = CreateDataAccess();
            using var dataAccessAfter = CreateDataAccess();
            using var dataAccessUpdate = CreateDataAccess();
            using var dataAccessUpdateAfter = CreateDataAccess();
            var actualModels = new List<TAccessModel>();

            Assert.AreEqual(accessModels.Count(), changedModels.Count());

            var insertModels = await dataAccess.InsertAsync(accessModels);

            Assert.IsNotNull(insertModels);
            await dataAccess.SaveChangesAsync();

            foreach (var item in insertModels)
            {
                var actualModel = await dataAccessAfter.GetByIdAsync(item.Id);

                Assert.IsNotNull(actualModel);
                Assert.IsTrue(item.AreEqualProperties(actualModel));
                actualModels.Add(actualModel);
            }

            var changedArray = changedModels.ToArray();

            for (int i = 0; i < actualModels.Count; i++)
            {
                var actualModel = actualModels[i];
                var changedModel = changedArray[i];

                actualModel.CopyFrom(changedModel, n => IgnoreUpdateProperties.Contains(n) == false);
            }

            var updateModels = await dataAccessUpdate.UpdateAsync(actualModels);

            Assert.IsNotNull(updateModels);
            await dataAccessUpdate.SaveChangesAsync();

            foreach (var item in updateModels)
            {
                var actualUpdateModel = await dataAccessUpdateAfter.GetByIdAsync(item.Id);

                Assert.IsNotNull(actualUpdateModel);
                Assert.IsTrue(item.AreEqualProperties(actualUpdateModel));
            }
        }
    }
}
//MdEnd