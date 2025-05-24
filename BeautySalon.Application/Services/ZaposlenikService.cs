using BeautySalon.Application.DTOs;
using BeautySalon.Application.Interfaces;
using BeautySalon.DataAccess.Repositories;
using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Services
{
    public class ZaposlenikService : IZaposlenikService
    {
        private readonly IZaposlenikRepository _repository;

        public ZaposlenikService(IZaposlenikRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ZaposlenikDto> GetAll()
        {
            return _repository.GetAll().Select(z => new ZaposlenikDto
            {
                KorisnikId = z.KorisnikId,
                Ime = z.Ime,
                Prezime = z.Prezime,
                Email = z.Email,
                BrojTelefona = z.BrojTelefona,
                Specijalizacija = z.Specijalizacija,
                Certifikat = z.Certifikat,
                DatumZaposlenja = z.DatumZaposlenja
            });
        }

        public ZaposlenikDto GetById(int id)
        {
            var z = _repository.GetById(id);
            if (z == null) return null;

            return new ZaposlenikDto
            {
                KorisnikId = z.KorisnikId,
                Ime = z.Ime,
                Prezime = z.Prezime,
                Email = z.Email,
                BrojTelefona = z.BrojTelefona,
                Specijalizacija = z.Specijalizacija,
                Certifikat = z.Certifikat,
                DatumZaposlenja = z.DatumZaposlenja
            };
        }

        public void Add(ZaposlenikDto zaposlenikDto)
        {
            var zaposlenik = new Zaposlenik
            {
                Ime = zaposlenikDto.Ime,
                Prezime = zaposlenikDto.Prezime,
                Email = zaposlenikDto.Email,
                BrojTelefona = zaposlenikDto.BrojTelefona,
                Specijalizacija = zaposlenikDto.Specijalizacija,
                Certifikat = zaposlenikDto.Certifikat,
                DatumZaposlenja = zaposlenikDto.DatumZaposlenja,
                Lozinka = "" // Postavi prema logici registracije ili ostavi prazno
            };

            _repository.Add(zaposlenik);
        }

        public void Update(ZaposlenikDto zaposlenikDto)
        {
            var zaposlenik = _repository.GetById(zaposlenikDto.KorisnikId);
            if (zaposlenik == null) return;

            zaposlenik.Ime = zaposlenikDto.Ime;
            zaposlenik.Prezime = zaposlenikDto.Prezime;
            zaposlenik.Email = zaposlenikDto.Email;
            zaposlenik.BrojTelefona = zaposlenikDto.BrojTelefona;
            zaposlenik.Specijalizacija = zaposlenikDto.Specijalizacija;
            zaposlenik.Certifikat = zaposlenikDto.Certifikat;
            zaposlenik.DatumZaposlenja = zaposlenikDto.DatumZaposlenja;

            _repository.Update(zaposlenik);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
