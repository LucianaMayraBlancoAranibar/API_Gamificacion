namespace Gamificacion_API.Models
{
    public class SanctionCreationRequest
    {
        public int IdSanction { get; set; }
        public int IdStudent { get; set; }
        //public string? StudentName { get; set; }
        //public string? StudentLastName { get; set; }
        public string SanctionDescription { get; set; }
        public int SanctionValue { get; set; }
        public int ResponsibleGestorId { get; set; } // ID del gestor o administrador 
    }
}
