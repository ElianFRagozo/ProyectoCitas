using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProyectoCItas.Models
{
    public class Cita
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PacienteId { get; set; }
        public string MedicoId { get; set; }
        public string Hora { get; set; }
        public DateTime Dia { get; set; }
        public string Especialidad { get; set; }
        public string Estado { get; set; } // Por ejemplo: "Pendiente", "Confirmada", "Cancelada"


    }
}
