using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ValOperationService
{
    private class OperationConfig
    {
        public Type ResultType { get; set; }
        public string Path { get; set; }
    }

    private static readonly Dictionary<string, OperationConfig> _operationConfigs = new Dictionary<string, OperationConfig>
    {
        { "ValOperations", new OperationConfig 
            { 
                ResultType = typeof(ValOperationPaged), 
                Path = "api/valoperation/valoperationssheach" 
            }
        },
        { "OtherOperation1", new OperationConfig 
            { 
                ResultType = typeof(OtherType1), 
                Path = "api/valoperation/otherpath1" 
            }
        },
        { "OtherOperation2", new OperationConfig 
            { 
                ResultType = typeof(OtherType2), 
                Path = "api/valoperation/otherpath2" 
            }
        },
        { "OtherOperation3", new OperationConfig 
            { 
                ResultType = typeof(OtherType3), 
                Path = "api/valoperation/otherpath3" 
            }
        },
        { "OtherOperation4", new OperationConfig 
            { 
                ResultType = typeof(OtherType4), 
                Path = "api/valoperation/otherpath4" 
            }
        }
    };

    public async Task<object> ExecuteOperation(string operationKey, object filter)
    {
        if (!_operationConfigs.TryGetValue(operationKey, out var config))
        {
            throw new InvalidOperationException($"Operação não configurada: {operationKey}");
        }

        var content = await http.post(config.Path, filter);
        var responseContent = content.ToString();
        
        // Usando o método correto que recebe o tipo como parâmetro
        var result = JsonConvert.DeserializeObject(responseContent, config.ResultType);
        
        return result;
    }
}

// Classe estática para definir as chaves de operação como constantes
public static class Operations
{
    public const string ValOperations = "ValOperations";
    public const string OtherOperation1 = "OtherOperation1";
    public const string OtherOperation2 = "OtherOperation2";
    public const string OtherOperation3 = "OtherOperation3";
    public const string OtherOperation4 = "OtherOperation4";
}

// Exemplo de uso:
// var result = (ValOperationPaged)await service.ExecuteOperation(Operations.ValOperations, filter);
