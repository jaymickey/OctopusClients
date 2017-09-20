using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IEnvironmentRepository : IFindByName<EnvironmentResource>, IGet<EnvironmentResource>, ICreate<EnvironmentResource>, IModify<EnvironmentResource>, IDelete<EnvironmentResource>, IGetAll<EnvironmentResource>
    {
        List<MachineResource> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatus = null,
            string commStyle = null,
            string tenantIds = null,
            string tenantTags = null);
        Task<EnvironmentsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatus = null,
            string commStyle = null,
            string tenantIds = null,
            string tenantTags = null);
        Task Sort(string[] environmentIdsInOrder);
        Task<EnvironmentEditor> CreateOrModify(string name);
        Task<EnvironmentEditor> CreateOrModify(string name, string description);
    }

    class EnvironmentRepository : BasicRepository<EnvironmentResource>, IEnvironmentRepository
    {
        public EnvironmentRepository(IOctopusAsyncClient client)
            : base(client, "Environments")
        {
        }

        public List<MachineResource> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatus = null,
            string commStyle = null,
            string tenantIds = null,
            string tenantTags = null)
        {
            var resources = new List<MachineResource>();

            Client.Paginate<MachineResource>(environment.Link("Machines"), new {
                skip,
                take,
                partialName,
                roles,
                isDisabled,
                healthStatus,
                commStyle,
                tenantIds,
                tenantTags
            }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public Task<EnvironmentsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatus = null,
            string commStyle = null,
            string tenantIds = null,
            string tenantTags = null)
        {
            return Client.Get<EnvironmentsSummaryResource>(Client.RootDocument.Link("EnvironmentsSummary"), new
            {
                ids,
                partialName,
                roles,
                isDisabled,
                healthStatus,
                commStyle,
                tenantIds,
                tenantTags
            });
        }

        public Task Sort(string[] environmentIdsInOrder)
        {
            return Client.Put(Client.RootDocument.Link("EnvironmentSortOrder"), environmentIdsInOrder);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name)
        {
            return new EnvironmentEditor(this).CreateOrModify(name);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name, string description)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description);
        }
    }
}
