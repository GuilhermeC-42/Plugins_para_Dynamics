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

namespace Checar_Pedido_Duplicado
{
    public class Detectar_Duplicatas_Pedido : IPlugin
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
                if (entity.LogicalName != "salesorder")
                {
                    return;
                }
                if (context.Depth > 1)
                {
                    return;
                }
                if (entity.Attributes.Contains("ordernumber"))
                {
                    string pedido = entity.GetAttributeValue<string>("ordernumber").ToString();
                    QueryExpression contactQuery = new QueryExpression("salesorder");
                    contactQuery.ColumnSet = new ColumnSet("ordernumber");
                    contactQuery.Criteria.AddCondition("ordernumber", ConditionOperator.Equal, pedido);
                    EntityCollection contactColl = service.RetrieveMultiple(contactQuery);
                    if (contactColl.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("ID de pedido já existente!"); ;

                    }
                }
            }
            #endregion
        }
    }
}