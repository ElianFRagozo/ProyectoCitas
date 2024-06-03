using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoCItas.Models;
using ProyectoCItas.Services.ProyectoCItas.Settings;



namespace ProyectoCItas.Services
{
    public class CitaService
    {
        private readonly IMongoCollection<Cita> _cita;

        public CitaService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _cita = database.GetCollection<Cita>("Cita");
        }
        public async Task CreateCitaAsync(Cita cita)
        {
            var newId = ObjectId.GenerateNewId();
            cita.Id = newId.ToString();

            await _cita.InsertOneAsync(cita);
        }
        public async Task<Cita> GetCitaEstadoAsync(string medicoId, DateTime dia, string hora)
        {
            return await _cita.Find(cita =>
                cita.Hora == hora && // Comparar las horas como strings
                cita.Dia.Date == dia.Date && // Comparar solo la fecha
                cita.Estado == "Confirmada" &&
                cita.MedicoId == medicoId)
            .FirstOrDefaultAsync();
        }
        public async Task<Cita> GetCitaByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _cita.Find(cita => cita.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task UpdateCitaAsync(Cita cita)
        {
            var filter = Builders<Cita>.Filter.Eq(c => c.Id, cita.Id);
            await _cita.ReplaceOneAsync(filter, cita);
        }
        public async Task<Cita> GetCitaIdAsync(string _citaId, string estado)
        {
            var objectId = ObjectId.Parse(_citaId);
            return await _cita.Find(cita => (cita.Id == objectId.ToString())).FirstOrDefaultAsync();
        }

        public async Task<List<Cita>> GetCitasPorDoctorYHoraAsync(string medicoId, string hora)
        {
            var filter = Builders<Cita>.Filter.And(
                Builders<Cita>.Filter.Eq(c => c.MedicoId, medicoId),
                Builders<Cita>.Filter.Eq(c => c.Hora, hora)
            );

            return await _cita.Find(filter).ToListAsync();
        }



        public async Task<List<Cita>> GetCitaAsync()
        {
            return await _cita.Find(patient => true).ToListAsync();
        }



    }

    namespace ProyectoCItas.Settings
    {
        public interface IMongoDatabaseSettings
        {
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }

        public class MongoDatabaseSettings : IMongoDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }
    }
}
