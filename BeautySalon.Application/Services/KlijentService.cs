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
    public class KlijentService : IKlijentService
    {
        private readonly IKlijentRepository _klijentRepository;

        public KlijentService(IKlijentRepository klijentRepository)
        {
            _klijentRepository = klijentRepository;
        }

        public IEnumerable<KlijentDto> GetAll()
        {
            return _klijentRepository.GetAll().Select(k => new KlijentDto
            {
                KorisnikId = k.KorisnikId,
                Ime = k.Ime,
                Prezime = k.Prezime,
                Email = k.Email,
                BrojTelefona = k.BrojTelefona,
                Zahtjevi = k.Zahtjevi,
                BrojBodova = k.BrojBodova
            });
        }

        public KlijentDto GetById(int id)
        {
            var k = _klijentRepository.GetById(id);
            return k == null ? null : new KlijentDto
            {
                KorisnikId = k.KorisnikId,
                Ime = k.Ime,
                Prezime = k.Prezime,
                Email = k.Email,
                BrojTelefona = k.BrojTelefona,
                Zahtjevi = k.Zahtjevi,
                BrojBodova = k.BrojBodova
            };
        }

        public void Add(KlijentDto klijentDto)
        {
            var klijent = new Klijent
            {
                Ime = klijentDto.Ime,
                Prezime = klijentDto.Prezime,
                Email = klijentDto.Email,
                BrojTelefona = klijentDto.BrojTelefona,
                Zahtjevi = klijentDto.Zahtjevi,
                BrojBodova = klijentDto.BrojBodova
            };
            _klijentRepository.Add(klijent);
        }

        public void Update(KlijentDto klijentDto)
        {
            var klijent = _klijentRepository.GetById(klijentDto.KorisnikId);
            if (klijent != null)
            {
                klijent.Ime = klijentDto.Ime;
                klijent.Prezime = klijentDto.Prezime;
                klijent.Email = klijentDto.Email;
                klijent.BrojTelefona = klijentDto.BrojTelefona;
                klijent.Zahtjevi = klijentDto.Zahtjevi;
                klijent.BrojBodova = klijentDto.BrojBodova;
                _klijentRepository.Update(klijent);
            }
        }

        public void Delete(int id)
        {
            _klijentRepository.Delete(id);
        }
    }

}
