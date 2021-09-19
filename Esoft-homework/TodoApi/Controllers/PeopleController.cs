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
            var person = _context.Family.Find(id); //find the central person
            string ASCII = person.Name + " "; //add its name to the output
            BuildFamilyRecursion(id, 0, 1, 1, ref ASCII); //recursively prepend or append ancestors and descendants
            return ASCII; //returns a family tree in the format:
                          //^ancestor2ofperson ^^ancestorofancestor1 ^^ancestorofancestor1 ^ancestor1ofperson person vchild1ofperson vvchildofchild1 vchild2ofperson
        }

        //made to recursively prepend or append ancestors and descendants of a person, id is the central persons id, direction defines if it is building ancestors,
        //descendants, or both, height and depth is how many generations up or down recursively, and the string reference is to acces the variable form outside the scope
        //of the recursive function
        private void BuildFamilyRecursion(int id, int direction, int height, int depth, ref string ASCII)
        {
            Person newPerson = null;
            if (direction >= 0) //if at the central person or building ancestors
            {
                var listofancestors = from r in _context.Relations //retrieve the list of ancestors
                                      where (r.FromPersonId == id)
                                      select r;
                listofancestors.ToList();

                foreach (Relation relative in listofancestors) //for each relation in the list of ancestors
                {
                    newPerson = null; //ensure newperson is empty
                    newPerson = _context.Family.Find(relative.ToPersonId);  //attempt to retrieve the ancestor
                    if (newPerson != null) // if an ancestor did exist
                    {
                        ASCII = ASCII.Insert(0, new string('^', height) + newPerson.Name + " "); //prepend the ancestors name and generation indicator to the result
                        BuildFamilyRecursion(relative.ToPersonId, 1, height + 1, depth, ref ASCII); //recursively prepend that persons ancestors too
                    }

                }
            }
            if(direction <= 0) //if at the central person or building descendants
            {
                var listofdescendants = from r in _context.Relations  //retrieve the list of descendants
                                      where (r.ToPersonId == id)
                                      select r;
                listofdescendants.ToList();

                foreach (Relation relative in listofdescendants) //for each relation in the list of descendants
                {
                    newPerson = null; //ensure newperson is empty
                    newPerson = _context.Family.Find(relative.FromPersonId); //attempt to retrieve the descendant
                    if (newPerson != null) //if a descendant did exist
                    {
                        ASCII += new string('v', depth) + newPerson.Name + " "; //append the descendants name and generation indicator to the result
                        BuildFamilyRecursion(relative.FromPersonId, -1, height, depth + 1, ref ASCII); //recursively append that persons descendants too
                    }
                }
            }
            return;
        }

        // POST: /People
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
            if (!PersonExists(id))
            {
                return NotFound();
            }

            _context.Family.Add(relative);
            await _context.SaveChangesAsync(); //save relative to database to generate relative.id
            _context.Relations.Add(RelatePeople(relative.Id, id)); //add parent/child relation to database
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPersonAndChildRelation), relative.Id);
        }

        // POST: api/People/parent/5
        [HttpPost("parent/{id}")]
        public async Task<ActionResult> PostPersonAndParentRelation([FromRoute] int id, [FromBody] Person relative)
        {
            if (!PersonExists(id))
            {
                return NotFound();
            }

            _context.Family.Add(relative);
            await _context.SaveChangesAsync(); //save relative to database to generate relative.id
            _context.Relations.Add(RelatePeople(id, relative.Id)); //add parent/child relation to database
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPersonAndParentRelation), relative.Id);
        }

        // POST: api/People/parent/5
        [HttpPost("grandparent/{id}")]
        public async Task<ActionResult> PostPersonAndGrandarentRelation([FromRoute] int id, [FromBody] Person relative)
        {
            if (!PersonExists(id))
            {
                return NotFound();
            }

            Person intermediate = new Person(); //create the unknown intermediate person
            intermediate.Name = "?";
            intermediate.BirthYear = 0;

            _context.Family.Add(intermediate); //add both the intermediate and the relative to database
            _context.Family.Add(relative);
            await _context.SaveChangesAsync(); //save to database to generate id
            _context.Relations.Add(RelatePeople(id, intermediate.Id)); //add relation from person to intermediate
            _context.Relations.Add(RelatePeople(intermediate.Id, relative.Id)); //add relation from intermediate to relative
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostPersonAndGrandarentRelation), relative.Id);
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

        private Relation RelatePeople(int id1, int id2)
        {
            Relation R = new Relation
            {
                FromPersonId = id1,
                ToPersonId = id2
            };
            return R;
        }
    }
}
