using CryptexAPI.Models.Wallets;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services;

public class SeedPhraseService : ISeedPhraseService
{
    private readonly IUnitOfWork _unitOfWork;

    public SeedPhraseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddBaseWords()
    {
        try
        {
            var words = new List<string>
            {
                "umbrella", "window", "elephant", "chair", "spaghetti", "notebook", "clover", "ocean",
                "aardvark",
                "chocolate",
                "eyebrow", "pigeon", "cup", "rose", "dragon", "cell", "fork", "bicycle", "lipstick", "corn",
                "cow", "flamingo", "ghost", "muffin", "paw", "windmill", "potato", "rainbow", "swamp", "whisk",
                "gnome", "spaceship", "wallet", "dinosaur", "elbow", "fiddle", "gorilla", "harp", "igloo",
                "jackal",
                "kiwi", "llama", "mango", "nugget", "octopus", "peanut", "quokka", "raccoon", "snail", "taco",
                "unicorn", "vampire", "wombat", "xylophone", "yak", "zebra", "atom", "banjo", "cactus",
                "dolphin",
                "echo", "flannel", "goblin", "hamburger", "iceberg", "jigsaw", "kaleidoscope", "lemon",
                "meadow",
                "nachos",
                "ostrich", "penguin", "quilt", "rooster", "scarecrow", "thimble", "underwear", "vortex",
                "waffle",
                "xenon",
                "yodel", "zucchini", "antelope", "bubbles", "caterpillar", "donkey", "eclipse", "firefly",
                "glacier", "hedgehog",
                "icicle", "jellyfish", "kitten", "lobster", "marshmallow", "noodles", "owl", "pumpkin",
                "quicksand",
                "raspberry",
                "skeleton", "tumbleweed", "urchin", "volcano", "walrus", "", "", "zeppelin", "accordion",
                "bagel",
                "camel", "daffodil", "earlobe", "flapjack", "grapefruit", "hamster", "inchworm",
                "jack-o-lantern",
                "kangaroo", "lighthouse",
                "mushroom", "nose", "otter", "pancake", "quail", "rhinoceros", "spatula", "turnip", "ukulele",
                "vampire",
                "wolverine", "xylophone", "yacht", "zipper", "armadillo", "bubblegum", "chimpanzee", "dumpling",
                "eyelash", "flamingo",
                "goldfish", "hickory", "ice cream", "jalapeno", "kiwi", "ladybug", "mailbox", "nugget",
                "oatmeal",
                "popsicle",
                "quartz", "rattlesnake", "sandwich", "tadpole", "umbrella", "volleyball", "waffle", "xylophone",
                "yogurt", "zeppelin"
            };

            var cleaned = words
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Distinct()
                .ToList();

            var SeedPhrase = new SeedPhrase()
            {
                SeedPhraseValues = cleaned
            };

            await _unitOfWork.SeedPhraseRepository.AddAsync(SeedPhrase);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<SeedPhrase> GetSeedPhraseBase()
    {
        try
        {
            var result = await _unitOfWork.SeedPhraseRepository.GetSingleByConditionAsync(e => e.SeedPhraseValues[0] == "umbrella");
            return result.Data;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get Seed Phrase base" + e.Message);
        }
    }
}
