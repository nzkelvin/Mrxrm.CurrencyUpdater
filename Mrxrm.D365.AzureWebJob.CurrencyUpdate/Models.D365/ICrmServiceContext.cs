#pragma warning disable SA1200 // Using directives must be placed correctly
using System.Linq;
using Microsoft.Xrm.Sdk;
#pragma warning restore SA1200 // Using directives must be placed correctly

namespace Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365
{
    public interface ICrmServiceContext
    {
#pragma warning disable SA1134 // Attributes must not share line
#pragma warning disable SA1411 // Attribute constructor must not use unnecessary parenthesis
        /// <summary>
        /// Gets a binding to the set of all <see cref="Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.Organization"/> entities.
        /// </summary>
        System.Linq.IQueryable<Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.Organization> OrganizationSet { [System.Diagnostics.DebuggerNonUserCode()] get; }

        /// <summary>
        /// Gets a binding to the set of all <see cref="Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.TransactionCurrency"/> entities.
        /// </summary>
        System.Linq.IQueryable<Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365.TransactionCurrency> TransactionCurrencySet { [System.Diagnostics.DebuggerNonUserCode()] get; }
#pragma warning restore SA1411 // Attribute constructor must not use unnecessary parenthesis
#pragma warning restore SA1134 // Attributes must not share line

        IQueryable<Entity> CreateQuery(string entityLogicalName);

        OrganizationResponse Execute(OrganizationRequest request);
    }
}