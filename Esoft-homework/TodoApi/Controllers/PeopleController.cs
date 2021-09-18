using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PersonContext _context;

        public PeopleController(PersonContext context)
        {
            _context = context;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Family.ToListAsync();
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Family.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // GET: api/People/relations
        [HttpGet("relations")]
        public async Task<ActionResult<IEnumerable<Relation>>> GetRelations()
        {
            return await _context.Relations.ToListAsync();
        }

        // POST: api/People
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostPerson(Person person)
        {
            _context.Family.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPerson), person.Id);
        }

        // POST: api/People/5/parent or api/People/5/child
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("parent/{id}")]
        public async Task<ActionResult> PostPersonAndRelation([FromRoute] int id, [FromBody] Person relative)
        {
            if (!PersonExists(id))
            {
                return NotFound();
            }

            _context.Family.Add(relative);
            await _context.SaveChangesAsync();
            _context.Relations.Add(RelatePeople(id, relative.Id, "parent"));
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPerson), relative.Id);
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> DeletePerson(int id)
        {
            var person = await _context.Family.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Family.Remove(person);
            await _context.SaveChangesAsync();

            return person;
        }

        // DELETE: api/People/relations/5
        [HttpDelete("relations/{id}")]
        public async Task<ActionResult<Relation>> DeleteRelation(int id)
        {
            var relation = await _context.Relations.FindAsync(id);
            if (relation == null)
            {
                return NotFound();
            }
           
            _context.Relations.Remove(relation);
            await _context.SaveChangesAsync();

            return relation;
        }

        private bool PersonExists(int id)
        {
            return _context.Family.Any(e => e.Id == id);
        }

        private Relation RelatePeople(int id1, int id2, string connection)
        {
            Relation R = new Relation();
            R.FromPersonId = id1;
            R.ToPersonId = id2;
            R.Connection = connection;

            return R;
        }
    }
}
