using BeautySalon.Application.DTOs;
using BeautySalon.Domain.Models;
using BeautySalon.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using BeautySalon.Application.Interfaces;

namespace BeautySalon.Application.Services
{
    public class TerminService : ITerminService
    {
        private readonly ITerminRepository _terminRepository;

        public TerminService(ITerminRepository terminRepository)
        {
            _terminRepository = terminRepository;
        }

        public IEnumerable<TerminDto> GetAll()
        {
            return _terminRepository.GetAll().Select(t => new TerminDto
            {
                TerminId = t.TerminId,
                DatumVrijeme = t.DatumVrijeme,
                Status = t.Status,
                KlijentId = t.KlijentId,
                ZaposlenikId = t.ZaposlenikId,
                KomentarZaposlenika = t.KomentarZaposlenika
            });
        }

        public TerminDto GetById(int id)
        {
            var t = _terminRepository.GetById(id);
            return t == null ? null : new TerminDto
            {
                TerminId = t.TerminId,
                DatumVrijeme = t.DatumVrijeme,
                Status = t.Status,
                KlijentId = t.KlijentId,
                ZaposlenikId = t.ZaposlenikId,
                KomentarZaposlenika = t.KomentarZaposlenika
            };
        }

        public void Add(CreateTerminDto termin)
        {
            var entity = new Termin
            {
                DatumVrijeme = termin.DatumVrijeme,
                Status = string.IsNullOrEmpty(termin.Status) ? "Zakazan" : termin.Status,
                KlijentId = termin.KlijentId,
                ZaposlenikId = termin.ZaposlenikId,
                KomentarZaposlenika = termin.KomentarZaposlenika
            };

            _terminRepository.Add(entity);
            _terminRepository.Save();
        }

        public void Update(TerminDto termin)
        {
            var entity = _terminRepository.GetById(termin.TerminId);
            if (entity == null)
                throw new Exception("Termin nije pronađen");

            entity.DatumVrijeme = termin.DatumVrijeme;
            entity.Status = termin.Status ?? entity.Status;
            entity.ZaposlenikId = termin.ZaposlenikId;
            entity.KomentarZaposlenika = termin.KomentarZaposlenika;

            _terminRepository.Update(entity);
            _terminRepository.Save();
        }




        public void Delete(int id)
        {
            _terminRepository.Delete(id);
            _terminRepository.Save(); // DODAJ OVO
        }


        public IEnumerable<TerminDto> GetByKlijentId(int klijentId)
        {
            return _terminRepository.GetByKlijentId(klijentId).Select(t => new TerminDto
            {
                TerminId = t.TerminId,
                DatumVrijeme = t.DatumVrijeme,
                Status = t.Status,
                KlijentId = t.KlijentId,
                ZaposlenikId = t.ZaposlenikId,
                KomentarZaposlenika = t.KomentarZaposlenika
            });
        }
    }
}