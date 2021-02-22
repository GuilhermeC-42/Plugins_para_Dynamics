using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Checar_CNPJ_Duplicado
{
    public class Detectar_Duplicatas : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory servicefactory =
             (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service =
            servicefactory.CreateOrganizationService(context.UserId);

            #region working code
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName != "account")
                {
                    return;
                }
                if (context.Depth > 1)
                {
                    return;
                }
                if (entity.Attributes.Contains("new_cnpj"))
                {
                    string cnpj = entity.GetAttributeValue<string>("new_cnpj").ToString();
                    QueryExpression contactQuery = new QueryExpression("account");
                    contactQuery.ColumnSet = new ColumnSet("new_cnpj");
                    contactQuery.Criteria.AddCondition("new_cnpj", ConditionOperator.Equal, cnpj);
                    EntityCollection contactColl = service.RetrieveMultiple(contactQuery);
                    if (contactColl.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("CNPJ já cadastrado no sistema!"); ;

                    }
                }
            }
            #endregion
        }
    }
}