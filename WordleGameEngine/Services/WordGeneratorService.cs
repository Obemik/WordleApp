using WordleGameEngine.Models;

namespace WordleGameEngine.Services;

public class WordGeneratorService
{
    private readonly Random _random;
    private readonly List<string> _defaultWords;

    public WordGeneratorService()
    {
        _random = new Random();
        _defaultWords = GetDefaultWords();
    }

    /// <summary>
    /// Генерує випадкове слово з заданого списку
    /// </summary>
    public string GenerateRandomWord(List<string> words)
    {
        if (words == null || words.Count == 0)
            throw new ArgumentException("Word list cannot be null or empty");

        return words[_random.Next(words.Count)].ToUpper();
    }

    /// <summary>
    /// Генерує випадкове слово з базового словника
    /// </summary>
    public string GenerateRandomWord()
    {
        return GenerateRandomWord(_defaultWords);
    }

    /// <summary>
    /// Перевіряє, чи є слово в базовому словнику
    /// </summary>
    public bool IsWordInDictionary(string word)
    {
        return _defaultWords.Contains(word.ToUpper());
    }

    /// <summary>
    /// Додає слово до базового словника
    /// </summary>
    public void AddWordToDictionary(string word)
    {
        if (!string.IsNullOrWhiteSpace(word) && word.Length == 5 && !_defaultWords.Contains(word.ToUpper()))
        {
            _defaultWords.Add(word.ToUpper());
        }
    }

    /// <summary>
    /// Видаляє слово з базового словника
    /// </summary>
    public bool RemoveWordFromDictionary(string word)
    {
        return _defaultWords.Remove(word.ToUpper());
    }

    /// <summary>
    /// Отримує список слів за базовим словником
    /// </summary>
    public List<string> GetDefaultWords()
    {
        return new List<string>
        {
            "ABOUT", "ABOVE", "ABUSE", "ACTOR", "ACUTE", "ADMIT", "ADOPT", "ADULT", "AFTER", "AGAIN",
            "AGENT", "AGREE", "AHEAD", "ALARM", "ALBUM", "ALERT", "ALIEN", "ALIGN", "ALIKE", "ALIVE",
            "ALLOW", "ALONE", "ALONG", "ALTER", "ANGEL", "ANGER", "ANGLE", "ANGRY", "APART", "APPLE",
            "APPLY", "ARENA", "ARGUE", "ARISE", "ARMED", "ARMOR", "ARRAY", "ARROW", "ASIDE", "ASSET",
            "AVOID", "AWAKE", "AWARD", "AWARE", "BADLY", "BAKER", "BASES", "BASIC", "BEACH", "BEGAN",
            "BEGIN", "BEING", "BELOW", "BENCH", "BILLY", "BIRTH", "BLACK", "BLAME", "BLANK", "BLAST",
            "BLIND", "BLOCK", "BLOOD", "BOARD", "BOOST", "BOOTH", "BOUND", "BRAIN", "BRAND", "BRASS",
            "BRAVE", "BREAD", "BREAK", "BREED", "BRIEF", "BRING", "BROAD", "BROKE", "BROWN", "BUILD",
            "BUILT", "BUYER", "CABLE", "CALIF", "CARRY", "CATCH", "CAUSE", "CHAIN", "CHAIR", "CHAOS",
            "CHARM", "CHART", "CHASE", "CHEAP", "CHECK", "CHEST", "CHIEF", "CHILD", "CHINA", "CHOSE",
            "CIVIL", "CLAIM", "CLASS", "CLEAN", "CLEAR", "CLICK", "CLIMB", "CLOCK", "CLOSE", "CLOUD",
            "COACH", "COAST", "COULD", "COUNT", "COURT", "COVER", "CRAFT", "CRASH", "CRAZY", "CREAM",
            "CRIME", "CROSS", "CROWD", "CROWN", "CRUDE", "CURVE", "CYCLE", "DAILY", "DANCE", "DATED",
            "DEALT", "DEATH", "DEBUT", "DELAY", "DEPTH", "DOING", "DOUBT", "DOZEN", "DRAFT", "DRAMA",
            "DRANK", "DREAM", "DRESS", "DRILL", "DRINK", "DRIVE", "DROVE", "DYING", "EAGER", "EARLY",
            "EARTH", "EIGHT", "ELITE", "EMPTY", "ENEMY", "ENJOY", "ENTER", "ENTRY", "EQUAL", "ERROR",
            "EVENT", "EVERY", "EXACT", "EXIST", "EXTRA", "FAITH", "FALSE", "FAULT", "FIBER", "FIELD",
            "FIGHT", "FINAL", "FIRST", "FIXED", "FLASH", "FLEET", "FLOOR", "FLUID", "FOCUS", "FORCE",
            "FORTH", "FORTY", "FORUM", "FOUND", "FRAME", "FRANK", "FRAUD", "FRESH", "FRONT", "FRUIT",
            "FULLY", "FUNNY", "GIANT", "GIVEN", "GLASS", "GLOBE", "GOING", "GRACE", "GRADE", "GRAND",
            "GRANT", "GRASS", "GRAVE", "GREAT", "GREEN", "GROSS", "GROUP", "GROWN", "GUARD", "GUESS",
            "GUEST", "GUIDE", "HAPPY", "HARSH", "HATE", "HEART", "HEAVY", "HENCE", "HENRY", "HORSE",
            "HOTEL", "HOUSE", "HUMAN", "IDEAL", "IMAGE", "INDEX", "INNER", "INPUT", "ISSUE", "JAPAN",
            "JIMMY", "JOINT", "JONES", "JUDGE", "KNOWN", "LABEL", "LARGE", "LASER", "LATER", "LAUGH",
            "LAYER", "LEARN", "LEASE", "LEAST", "LEAVE", "LEGAL", "LEVEL", "LEWIS", "LIGHT", "LIMIT",
            "LINKS", "LIVES", "LOCAL", "LOGIC", "LOOSE", "LOWER", "LUCKY", "LUNCH", "LYING", "MAGIC",
            "MAJOR", "MAKER", "MARCH", "MARIA", "MATCH", "MAYBE", "MAYOR", "MEANT", "MEDIA", "METAL",
            "MIGHT", "MINOR", "MINUS", "MIXED", "MODEL", "MONEY", "MONTH", "MORAL", "MOTOR", "MOUNT",
            "MOUSE", "MOUTH", "MOVED", "MOVIE", "MUSIC", "NEEDS", "NEVER", "NEWLY", "NIGHT", "NOISE",
            "NORTH", "NOTED", "NOVEL", "NURSE", "OCCUR", "OCEAN", "OFFER", "OFTEN", "ORDER", "OTHER",
            "OUGHT", "PAINT", "PANEL", "PAPER", "PARTY", "PEACE", "PETER", "PHASE", "PHONE", "PHOTO",
            "PIANO", "PIECE", "PILOT", "PITCH", "PLACE", "PLAIN", "PLANE", "PLANT", "PLATE", "POINT",
            "POUND", "POWER", "PRESS", "PRICE", "PRIDE", "PRIME", "PRINT", "PRIOR", "PRIZE", "PROOF",
            "PROUD", "PROVE", "QUEEN", "QUICK", "QUIET", "QUITE", "RADIO", "RAISE", "RANGE", "RAPID",
            "RATIO", "REACH", "READY", "REALM", "REBEL", "REFER", "RELAX", "RELAY", "RIDER", "RIDGE",
            "RIGHT", "RIGID", "RIVER", "ROBOT", "ROCKY", "ROGER", "ROMAN", "ROUGH", "ROUND", "ROUTE",
            "ROYAL", "RURAL", "SCALE", "SCENE", "SCOPE", "SCORE", "SENSE", "SERVE", "SETUP", "SEVEN",
            "SHALL", "SHAPE", "SHARE", "SHARP", "SHEET", "SHELF", "SHELL", "SHIFT", "SHINE", "SHIRT",
            "SHOCK", "SHOOT", "SHORT", "SHOWN", "SIDES", "SIGHT", "SIGN", "SILLY", "SINCE", "SIXTH",
            "SIXTY", "SIZED", "SKILL", "SLEEP", "SLIDE", "SMALL", "SMART", "SMILE", "SMITH", "SMOKE",
            "SOLID", "SOLVE", "SORRY", "SOUND", "SOUTH", "SPACE", "SPARE", "SPEAK", "SPEED", "SPEND",
            "SPENT", "SPLIT", "SPOKE", "SPORT", "STAFF", "STAGE", "STAKE", "STAND", "START", "STATE",
            "STEAM", "STEEL", "STEEP", "STEER", "STICK", "STILL", "STOCK", "STONE", "STOOD", "STORE",
            "STORM", "STORY", "STRIP", "STUCK", "STUDY", "STUFF", "STYLE", "SUGAR", "SUITE", "SUPER",
            "SWEET", "TABLE", "TAKEN", "TASTE", "TAXES", "TEACH", "TEAMS", "TEETH", "TERRY", "TEXAS",
            "THANK", "THEFT", "THEIR", "THEME", "THERE", "THESE", "THICK", "THING", "THINK", "THIRD",
            "THOSE", "THREE", "THREW", "THROW", "THUMB", "TIGER", "TIGHT", "TIMES", "TIRED", "TITLE",
            "TODAY", "TOPIC", "TOTAL", "TOUCH", "TOUGH", "TOWER", "TRACK", "TRADE", "TRAIN", "TREAT",
            "TREND", "TRIAL", "TRIBE", "TRICK", "TRIED", "TRIES", "TRUCK", "TRULY", "TRUNK", "TRUST",
            "TRUTH", "TWICE", "UNDER", "UNDUE", "UNION", "UNITY", "UNTIL", "UPPER", "UPSET", "URBAN",
            "USAGE", "USUAL", "VALID", "VALUE", "VIDEO", "VIRUS", "VISIT", "VITAL", "VOCAL", "VOICE",
            "WASTE", "WATCH", "WATER", "WHEEL", "WHERE", "WHICH", "WHILE", "WHITE", "WHOLE", "WHOSE",
            "WOMAN", "WOMEN", "WORLD", "WORRY", "WORSE", "WORST", "WORTH", "WOULD", "WRITE", "WRONG",
            "WROTE", "YOUNG", "YOUTH"
        };
    }

    /// <summary>
    /// Отримує кількість слів у словнику
    /// </summary>
    public int GetDictionarySize()
    {
        return _defaultWords.Count;
    }
}