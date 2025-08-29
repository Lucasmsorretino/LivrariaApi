namespace SuperHeroApi
{
    public interface IDependencia_obj
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    public class Dependencia_obj : IDependencia_obj
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
    
}