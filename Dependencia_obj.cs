// Define o namespace principal da aplicação LivrariaApi
namespace LivrariaApi
{
    // Declaração da interface IDependencia_obj que define o contrato para objetos de dependência
    // Interfaces são contratos que definem quais propriedades e métodos uma classe deve implementar
    public interface IDependencia_obj
    {
        // Propriedade que define o nome do objeto de dependência
        // Get e set permitem leitura e escrita da propriedade
        public string Nome { get; set; }
        
        // Propriedade que define a descrição do objeto de dependência
        // Get e set permitem leitura e escrita da propriedade
        public string Descricao { get; set; }
    } // Fim da interface IDependencia_obj

    // Declaração da classe Dependencia_obj que implementa a interface IDependencia_obj
    // Esta classe fornece a implementação concreta dos membros definidos na interface
    public class Dependencia_obj : IDependencia_obj
    {
        // Implementação da propriedade Nome conforme definido na interface
        // Inicializada com string.Empty para evitar valores null por padrão
        public string Nome { get; set; } = string.Empty;
        
        // Implementação da propriedade Descricao conforme definido na interface
        // Inicializada com string.Empty para evitar valores null por padrão
        public string Descricao { get; set; } = string.Empty;
    } // Fim da classe Dependencia_obj
    
} // Fim do namespace LivrariaApi