using DbTextEditor.Model.DAL;
using DbTextEditor.Model.DAL.Interfaces;
using DbTextEditor.Model.Entities;
using DbTextEditor.Model.Infrastructure.Interfaces;
using DbTextEditor.Shared;
using DbTextEditor.Shared.Storage;

namespace DbTextEditor.Model.Infrastructure
{
    public class LocalFilesAdapter : IFilesAdapter
    {
        private readonly ILocalFilesRepository _repository;

        public LocalFilesAdapter(ILocalFilesRepository repository)
        {
            _repository = repository;
        }

        public void Save(FileDto model)
        {
            var entity = Map(model);
            if (!_repository.Exists(entity.Path))
            {
                _repository.Create(entity);
            }
            else
            {
                _repository.Update(entity);
            }
        }

        public FileDto Open(string fileName) =>
            _repository.Exists(fileName)
                ? Map(_repository.Get(fileName))
                : default(FileDto);

        public bool Exists(string fileName) => _repository.Exists(fileName);

        private static LocalFileEntity Map(FileDto model) =>
            new LocalFileEntity
            {
                Path = model.FileName,
                Contents = model.Contents
            };

        private static FileDto Map(LocalFileEntity entity) =>
            new FileDto
            {
                FileName = entity.Path,
                Contents = entity.Contents
            };
    }
}