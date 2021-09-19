using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EsoftApi.Models;

namespace EsoftApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly Context _context;

        public PeopleController(Context context)
        {
            _context = context;
        }

        // GET: /People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Family.ToListAsync();
        }

        // GET: /People/5
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

        // GET: /People/relations
        [HttpGet("relations")]
        public async Task<ActionResult<IEnumerable<Relation>>> GetRelations()
        {
            return await _context.Relations.ToListAsync();
        }

        // GET: /People/relations/5
        [HttpGet("relations/{id}")]
        public async Task<ActionResult<Relation>> GetRelation(int id)
        {
            var relation = await _context.Relations.FindAsync(id);

            if (relation == null)
            {
                return NotFound();
            }

            return relation;
        }

        // GET: /People/FamilyOf/5
        [HttpGet("familyof/{id}")]
        public string GetFamilyOf(int id)
        {
            var person = _context.Family.Find(id);
            string ASCII = person.Name + " ";
            BuildFamilyRecursion(id, 0, 1, 1, ref ASCII);
            return ASCII;
        }

        private void BuildFamilyRecursion(int id, int direction, int height, int depth, ref string ASCII)
        {
            var listofrelatives = from r in _context.Relations
                                       where (r.FromPersonId == id)
                                       select r;
            listofrelatives.ToList();
            Person newPerson = null;
            foreach (Relation relative in listofrelatives)
            {
                newPerson = null;
                if ((relative.Connection == "parent") && (direction >= 0))
                {
                    newPerson = _context.Family.Find(relative.ToPersonId);
                    if (newPerson != null)
                    {
                        ASCII = ASCII.Insert(0, new string('^', height) + newPerson.Name + " ");
                        BuildFamilyRecursion(relative.ToPersonId, 1, height+1, depth, ref ASCII);
                    }

                }
                newPerson = null;
                if ((relative.Connection == "child") && (direction <= 0))
                {
                    newPerson = _context.Family.Find(relative.ToPersonId);
                    if (newPerson != null)
                    {
                        ASCII += new string('v', depth) + newPerson.Name + " ";
                        BuildFamilyRecursion(relative.ToPersonId, -1, height, depth+1, ref ASCII);
                    }
                }
            }
            return;
        }

        // POST: /People
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostPerson(Person person)
        {
            _context.Family.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPerson), person.Id);
        }

        // POST: /People/child/5
        [HttpPost("child/{id}")]
        public async Task<ActionResult> PostPersonAndChildRelation([FromRoute] int id, [FromBody] Person relative)
        {
            return await PostPersonAndGenericRelation(id, relative, "child");
        }

        // POST: api/People/parent/5
        [HttpPost("parent/{id}")]
        public async Task<ActionResult> PostPersonAndParentRelation([FromRoute] int id, [FromBody] Person relative)
        {
            return await PostPersonAndGenericRelation(id, relative, "parent");
        }

        // POST: api/People/parent/5
        [HttpPost("grandparent/{id}")]
        public async Task<ActionResult> PostPersonAndGrandarentRelation([FromRoute] int id, [FromBody] Person relative)
        {
            if (!PersonExists(id))
            {
                return NotFound();
            }

            Person intermediate = new Person();
            intermediate.Name = "?";
            intermediate.BirthYear = 0;

            _context.Family.Add(intermediate);
            _context.Family.Add(relative);
            await _context.SaveChangesAsync();
            _context.Relations.Add(RelatePeople(id, intermediate.Id, "parent"));
            _context.Relations.Add(RelatePeople(intermediate.Id, relative.Id, "parent"));
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPerson), relative.Id);
        }

        private async Task<ActionResult> PostPersonAndGenericRelation(int id, Person relative, string connection)
        {
            if (!PersonExists(id))
            {
                return NotFound();
            }

            _context.Family.Add(relative);
            await _context.SaveChangesAsync();
            _context.Relations.Add(RelatePeople(id, relative.Id, connection));
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
