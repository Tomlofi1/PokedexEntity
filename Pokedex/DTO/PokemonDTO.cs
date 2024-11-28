namespace PokemonReviewApp.DTO
{
    public class PokemonDTO
    {
        //DTO sao essencialmente um metodo de ter certeza que vc nao esta retornando todos os dados //
        //DTO existem para nao expor dados pessoais coisas assim //
        //DTO basicamente isso aqui e pra limitar o tanto de dados que vc quer mostrar para as pessoas e o DTO serve justamente para isso//

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
