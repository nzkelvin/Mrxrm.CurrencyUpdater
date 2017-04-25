using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365
{
    public interface ICrmServiceContext
    {
        /// <summary>
        /// Gets a binding to the set of all <see cref="Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.Organization"/> entities.
        /// </summary>
        System.Linq.IQueryable<Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.Organization> OrganizationSet { [System.Diagnostics.DebuggerNonUserCode()] get; }

        /// <summary>
        /// Gets a binding to the set of all <see cref="Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.TransactionCurrency"/> entities.
        /// </summary>
        System.Linq.IQueryable<Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.TransactionCurrency> TransactionCurrencySet { [System.Diagnostics.DebuggerNonUserCode()] get; }

        IQueryable<Entity> CreateQuery(string entityLogicalName);
        OrganizationResponse Execute(OrganizationRequest request);
    }
}