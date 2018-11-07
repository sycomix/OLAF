﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAF
{
    public class TextArtifact : Artifact
    {
        #region Constructors
        public TextArtifact(long id, List<string> rawText) : base(id)
        {
            Text = new List<string>();
            Urls = new List<string>();
           
            Sentiment = new Dictionary<string, double?>(rawText.Count);
            HasEmoticon = new Dictionary<string, bool>();
            HasProfanity = new Dictionary<string, bool?>(rawText.Count);
            HasIdentityHateWords = new Dictionary<string, bool?>();
            HasHatePhrases = new Dictionary<string, bool?>(rawText.Count);

            Languages = new Dictionary<string, string>(rawText.Count);
        
            foreach (string t in rawText)
            {
                var urls = t.Split(new[] { ' ','\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.StartsWith("http:") || w.StartsWith("hitp:") 
                    || w.StartsWith("https:") || w.StartsWith("hitps:")
                    || w.StartsWith("https/") || w.StartsWith("http/")
                    );
                if (urls.Count() > 0)
                {
                    Urls.AddRange(urls);
                    continue;
                }

                string ant = GetAlphaNumericString(t);
                ant = string.Join(" ", 
                    ant.Split(new[] { ' ','\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length >= 3 || Pipeline.Dictionaries["words_en_3grams"].Contains(w)))
                    .Trim();

                if (ant.IsEmpty())
                {
                    continue;
                }
                else if (Sentiment.Keys.Contains(ant))
                {
                    continue;
                }
                Sentiment.Add(ant, null);
                HasEmoticon.Add(ant, ant.Split(new[] { ' ', '\r', '\n'}).Any(a => emoticonWords.Contains(a)));
                HasProfanity.Add(ant, ant.Split(new[] { ' ', '\r', '\n' }).Any(a => profanityWords.Contains(a)));
                HasIdentityHateWords.Add(ant, ant.Split(new[] { ' ', '\r', '\n' }).Any(a => identityHateWords.Contains(a)));
                HasHatePhrases.Add(ant, hatePhrases.Contains(ant));
                Text.Add(ant);
            }
            Debug("Added text artifact {0} with Urls {1}.", Text, Urls);
        }
        #endregion

        #region Properties
        public List<string> RawText { get; }

        public List<string> Text { get; protected set; }

        public List<string> Urls { get; protected set; }

        public Dictionary<string, double?> Sentiment { get; protected set; }

        public Dictionary<string, bool> HasEmoticon { get; protected set; }

        public Dictionary<string, bool> HasNegativeEmotionWords { get; protected set; }

        public Dictionary<string, bool?> HasProfanity { get; protected set; }

        public Dictionary<string, bool?> HasIdentityHateWords { get; protected set; }

        public Dictionary<string, bool?> HasHatePhrases { get; protected set; }

        public Dictionary<string, string> Languages { get; protected set; }
        #endregion

        #region Methods
        public static string GetAlphaNumericString(string s)
        {
            StringBuilder an = new StringBuilder(s.Length);
            char p = ' ';
            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c))
                {
                    an.Append(c);
                    p = c;
                }
                else if (char.IsPunctuation(c) && char.IsLetterOrDigit(p))
                {
                    an.Append(c);
                    p = c;
                }
                else if (char.IsWhiteSpace(c) && (char.IsLetterOrDigit(p) || char.IsPunctuation(p)))
                {
                    an.Append(c);
                    p = c;
                }
                else continue;

            }
            return an.ToString();
        }
        #endregion

        #region Fields

        #region Words
        protected static string[] slangWords = @"121	one to one
            a/s/l	age, sex, location
            adn	any day now
            afaik	as far as I know
            afk	away from keyboard
            aight	alright
            alol	actually laughing out loud
            b4	before
            b4n	bye for now
            bak	back at the keyboard
            bf	boyfriend
            bff	best friends forever
            bfn	bye for now
            bg	big grin
            bta	but then again
            btw	by the way
            cid	crying in disgrace
            cnp	continued in my next post
            cp	chat post
            cu	see you
            cul	see you later
            cul8r	see you later 
            cya	bye
            cyo	see you online 
            dbau	doing business as usual 
            fud	fear, uncertainty, and doubt 
            fwiw	for what it's worth 
            fyi	for your information
            g	grin 
            g2g	got to go 
            ga	go ahead 
            gal	get a life 
            gf	girlfriend 
            gfn	gone for now
            gmbo	giggling my butt off 
            gmta	great minds think alike 
            h8	hate
            hagn	have a good night 
            hdop	help delete online predators 
            hhis	hanging head in shame 
            iac	in any case 
            ianal	I am not a lawyer
            ic	I see 
            idk	I don't know 
            imao	in my arrogant opinion
            imnsho	in my not so humble opinion 
            imo	in my opinion 
            iow	in other words 
            ipn	I’m posting naked 
            irl	in real life 
            jk	just kidding
            l8r	later
            ld	later, dude 
            ldr	long distance relationship 
            llta	lots and lots of thunderous applause 
            lmao	laugh my ass off
            lmirl	let's meet in real life 
            lol	laugh out loud
            ltr	longterm relationship 
            lulab	love you like a brother 
            lulas	love you like a sister 
            luv	love
            m/f	male or female 
            m8	mate
            milf	mother I would like to fuck
            oll	online love 
            omg	oh my god
            otoh	on the other hand 
            pir	parent in room
            ppl	people
            r	are
            rofl	roll on the floor laughing
            rpg	role playing games
            ru	are you
            shid	slaps head in disgust
            somy	sick of me yet
            sot	short of time 
            thanx	thanks
            thx	thanks
            ttyl	talk to you later 
            u	you
            ur	you are
            uw	you’re welcome 
            wb	welcome back 
            wfm	works for me 
            wibni	wouldn't it be nice if 
            wtf	what the fuck
            wtg	way to go
            wtgp	want to go private
            ym	young man".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();


        protected static string[] hedgeWords = {"almost", "apparent", "apparently", "appear", "appeared", "appears",
          "approximately", "argue", "argued", "argues", "around", "assume",
          "assumed", "broadly", "certain amount", "certain extent",
          "certain level", "claim", "claimed", "claims", "doubt", "doubtful",
          "essentially", "estimate", "estimated", "fairly", "feel", "feels",
          "felt", "frequently", "from my perspective", "from our perspective",
          "from this perspective", "generally", "guess", "in general",
          "in most cases", "in most instances", "in my opinion", "in my view",
          "in our opinion", "in our view", "in this view", "indicate",
          "indicated", "indicates", "largely", "likely", "mainly", "may",
          "maybe", "might", "mostly", "often", "on the whole", "ought",
          "perhaps", "plausible", "plausibly", "possible", "possibly",
          "postulate", "postulated", "postulates", "presumable", "presumably",
          "probable", "probably", "quite", "rather", "relatively", "roughly",
          "seems", "should", "sometimes", "somewhat", "suggest", "suggested",
          "suggests", "suppose", "supposed", "supposes", "suspect", "suspects",
          "tend to", "tended to", "tends to", "think", "thinking", "thought",
          "to my knowledge", "typical", "typically", "uncertain", "uncertainly",
          "unclear", "unclearly", "unlikely", "usually" };

        protected static string[] profanityWords = { "damn", "dyke", "fuck", "shit", "ahole", "amcik", "andskota", "anus",
            "arschloch", "arse", "ash0le", "ash0les", "asholes", "ass", "Ass Monkey", "Assface",
            "assh0le", "assh0lez", "asshole", "assholes", "assholz", "assrammer", "asswipe", "ayir",
            "azzhole", "b00b", "b00bs", "b17ch", "b1tch", "bassterds", "bastard",
            "bastards", "bastardz", "basterds", "basterdz", "bi7ch", "Biatch", "bitch", "bitch",
            "bitches", "Blow Job", "blowjob", "boffing", "boiolas", "bollock", "boobs", "breasts",
            "buceta", "butt-pirate", "butthole", "buttwipe", "c0ck", "c0cks",
            "c0k", "cabron", "Carpet Muncher", "cawk", "cawks", "cazzo", "chink", "chraa", "chuj",
            "cipa", "clit", "Clit", "clits", "cnts", "cntz", "cock", "cock-head", "cock-sucker",
            "Cock", "cockhead", "cocks", "CockSucker", "crap", "cum", "cunt",
            "cunt", "cunts", "cuntz", "d4mn", "daygo", "dego", "dick", "dick", "dike", "dild0",
            "dild0s", "dildo", "dildos", "dilld0", "dilld0s", "dirsa", "dominatricks", "dominatrics",
            "dominatrix", "dupa", "dyke", "dziwka", "ejackulate", "ejakulate", "Ekrem", "Ekto", "enculer",
            "enema", "f u c k", "f u c k e r", "faen", "fag", "fag", "fag1t", "faget",
            "fagg1t", "faggit", "faggot", "fagit", "fags", "fagz", "faig", "faigs", "fanculo", "fanny",
            "fart", "fatass", "fcuk", "feces", "feg", "Felcher", "ficken", "fitt", "Flikker", "flipping the bird",
            "foreskin", "Fotze", "fuck", "fucker", "fuckin", "fucking", "fucks", "Fudge Packer", "fuk", "fuk",
            "Fukah", "Fuken", "fuker", "Fukin", "Fukk", "Fukkah", "Fukker", "Fukkin", "futkretzn", "fux0r",
            "g00k", "gay", "gayboy", "gaygirl", "gays", "gayz", "God-damned", "gook",
            "guiena", "h00r", "h0ar", "h0r", "h0re", "h4x0r", "hell", "hells", "helvete", "hoar", "hoer",
            "hoer", "honkey", "hoore", "hore", "Huevon", "hui", "injun", "jackoff", "jap", "japs", "jerk-off",
            "jisim", "jism", "jiss", "jizm", "jizz", "kanker", "kawk", "kike", "klootzak", "knob", "knobs",
            "knobz", "knulle", "kraut", "kuk", "kuksuger", "kunt", "kunts", "kuntz", "Kurac", "kurwa", "kusi",
            "kyrpa", "l3i+ch", "l3itch", "lesbian", "Lesbian", "lesbo", "Lezzian",
            "Lipshitz", "mamhoon", "masochist", "masokist", "massterbait", "masstrbait", "masstrbate",
            "masterbaiter", "masterbat", "masterbat3", "masterbate", "masterbates", "masturbat", "masturbate",
            "merd", "mibun", "mofo", "monkleigh", "Motha Fucker", "Motha Fuker", "Motha Fukkah", "Motha Fukker",
            "mother-fucker", "Mother Fucker", "Mother Fukah", "Mother Fuker", "Mother Fukker", "motherfucker",
            "mouliewop", "muie", "mulkku", "muschi", "Mutha Fucker", "Mutha Fukah", "Mutha Fuker",
            "Mutha Fukkah", "Mutha Fukker", "n1gr", "nastt", "nazi", "nazis", "nepesaurio", "nigga", "nigger",
            "nigger", "nigger;", "nigur;", "niiger;", "niigr;", "nutsack", "orafis", "orgasim;", "orgasm", "orgasum",
            "oriface", "orifice", "orifiss", "orospu", "p0rn", "packi", "packie", "packy", "paki", "pakie", "paky",
            "paska", "pecker", "peeenus", "peeenusss", "peenus", "peinus", "pen1s", "penas", "penis", "penis-breath",
            "penus", "penuus", "perse", "Phuc", "phuck", "Phuck",  "Phuker", "Phukker", "picka", "pierdol", "pillu",
            "pimmel", "pimpis", "piss", "pizda", "polac", "polack", "polak",  "poontsee", "poop", "porn", "pr0n",
            "pr1c", "pr1ck", "pr1k", "preteen", "pula", "pule", "pusse", "pussee", "pussy", "puto", "puuke", "puuker",
            "qahbeh", "queef", "queer", "queers", "queerz", "qweers", "qweerz", "qweir", "rautenberg",
            "rectum", "retard", "sadist", "scank", "schaffer", "scheiss", "schlampe", "schlong", "schmuck", "screw",
            "screwing", "scrotum", "semen", "sex", "sexy", "sh!t", "Sh!t", "sh!t", "sh1t", "sh1ter", "sh1ts", "sh1tter",
            "sh1tz", "sharmuta", "shemale", "shi+", "shipal", "shit", "shits", "shitter", "Shitty", "Shity", "shitz",
            "shiz", "Shyt", "Shyte", "Shytty",  "skanck", "skank", "skankee", "skankey", "skanks", "Skanky", "skribz",
            "skurwysyn", "slut", "sluts", "Slutty", "slutz", "son-of-a-bitch", "sphencter", "spic", "spierdalaj",
            "splooge", "suka", "teets", "teez", "testical", "testicle", "testicle", "tit", "tits", "titt", "titt",
            "turd", "twat", "va1jina", "vag1na", "vagiina", "vagina", "vaj1na", "vajina", "vittu",
            "vulva", "w00se", "w0p", "wank", "wank", "wetback", "wh00r", "wh0re", "whoar", "whore",
            "wichser", "wop", "xrated", "xxx", "Lipshits", "Mother Fukkah", "zabourah", "Phuk", "Poonani",
            "puta", "recktum", "sharmute", "Shyty", "smut", "vullva", "yed"};

        protected static string[] negativeEmotionWordLabels = "negative-fear;sadness;general-dislike;ingratitude;shame;compassion;humility;despair;anxiety;daze".Split(';');

        protected static string[] negativeEmotionWords = @"affright;aggrieve;abhor;;abase;affectionate;abase;abject;afraid;bedaze
                afraid;bad;abhorrent;;abash;caring;chagrin;abjectly;anxious;daze
                alarm;bereaved;abominably;;abashed;commiserate;demeaning;baffled;anxiously;dazed
                alarmed;bereft;abominate;;ashamed;compassionate;demeaningly;balked;apprehensive;dazzling
                alert;blue;afraid;;awkward;condole_with;embarrassed;cynical;apprehensively;dazzlingly
                anxious;bored;aggravate;;black;excusable;humble;defeat;brood;fulgurant
                anxiously;cast_down;aggravated;;broken;feel_for;humbling;defeated;concern;fulgurous
                appal;cheerless;aggressive;;chagrin;fond;humiliate;demoralized;concerned;stun
                appall;cheerlessly;alien;;chagrined;forgivable;humiliated;despair;discomfit;stunned
                apprehensive;contrite;alienate;;confuse;forgive;humiliating;despairing;discomfited;stupefied
                apprehensively;contritely;alienated;;confused;lovesome;humiliatingly;despairingly;discompose;stupid
                atrocious;dark;amok;;confusedly;merciful;mortified;desperate;disconcert;
                awful;deject;amuck;;confusing;mercifully;mortify;despondently;disquieted;
                awfully;demoralising;anger;;consternate;pity;mortifying;disappointed;distress;
                bashfully;demoralize;angered;;discomfit;showing_mercy;self-deprecating;discomfited;distressed;
                browbeaten;demoralized;angrily;;discomfited;sympathize;;discourage;distressful;
                bullied;demoralizing;angry;;discompose;sympathize_with;;discouraged;distressfully;
                chill;deplorable;annoy;;disconcert;tender;;discouraging;distressing;
                chilling;deplorably;annoyed;;discreditably;tenderly;;disheartened;distressingly;
                cliff-hanging;depress;annoying;;discredited;venial;;dispiritedly;disturbed;
                cowed;depressed;anomic;;disgraced;warm;;foiled;disturbing;
                cower;depressing;avaricious;;disgraceful;with_mercy;;frustrated;dwell;
                crawl;depressive;baffled;;disgracefully;;;hopeless;dysphoric;
                creep;desolate;balked;;dishonorably;;;hopelessly;edgy;
                cringe;despairingly;bedevil;;dishonored;;;misanthropic;embarrassed;
                cruel;despondent;begrudge;;dishonourably;;;misanthropical;fidgety;
                cruelly;despondently;begrudging;;disordered;;;overcome;fretful;
                dash;dingy;belligerent;;embarrass;;;pessimistic;high-strung;
                daunt;disconsolate;belligerently;;embarrassed;;;pessimistically;impatient;
                diffident;discouraged;bother;;embarrassing;;;reconcile;impatiently;
                diffidently;disheartened;bothersome;;embarrassingly;;;resign;in_suspense;
                dire;disheartening;brood;;flurry;;;resigned;insecure;
                direful;dismal;choleric;;guilty;;;resignedly;insecurely;
                dismay;dismay;churn_up;;hangdog;;;submit;interest;
                dread;dispirit;contemn;;humble;;;thwarted;itchy;
                dreaded;dispirited;covet;;humiliate;;;unhopeful;jittery;
                dreadful;dispiriting;covetous;;humiliated;;;;jumpy;
                dreadfully;distressed;covetously;;ignominious;;;;nervous;
                fawn;doleful;crucify;;ignominiously;;;;nervy;
                fear;dolefully;despise;;inglorious;;;;occupy;
                fearful;dolorous;despiteful;;ingloriously;;;;overstrung;
                fearfully;dolourous;detached;;mortified;;;;painfully;
                fearsome;down;detest;;mortify;;;;perturbing;
                fright;downcast;detestable;;mortifying;;;;raring;
                frighten;downhearted;detestably;;opprobrious;;;;restive;
                frighten_away;downtrodden;devil;;put_off;;;;restless;
                frighten_off;drab;disaffect;;scandalous;;;;solicitous;
                frightened;drear;disapprove;;self-conscious;;;;solicitously;
                frightening;dreary;disapproving;;self-consciously;;;;troubling;
                frighteningly;dysphoric;discouraged;;shame;;;;uneasily;
                frightful;execrable;discouraging;;shamed;;;;uneasy;
                grovel;forlorn;disdain;;shamefaced;;;;unhappy;
                hangdog;forlornly;disgust;;shamefacedly;;;;unquiet;
                hardhearted;get_down;disgusted;;shameful;;;;untune;
                heartless;gloomful;disgustedly;;shamefully;;;;upset;
                heartlessly;gloomily;disgustful;;sheepish;;;;uptight;
                hesitantly;glooming;disgusting;;shocking;;;;with_impatience;
                hesitatingly;gloomy;disgustingly;;sticky;;;;worried;
                hideous;glum;disincline;;unenviable;;;;worrisome;
                hideously;godforsaken;disinclined;;untune;;;;worry;
                horrendous;grief-stricken;dislikable;;upset;;;;worrying;
                horrible;grieve;dislike;;;;;;worryingly;
                horribly;grieving;disoriented;;;;;;;
                horrid;grievous;displease;;;;;;;
                horridly;grievously;displeased;;;;;;;
                horrific;grim;displeasing;;;;;;;
                horrified;guilty;displeasingly;;;;;;;
                horrify;hangdog;distasteful;;;;;;;
                horrifying;hapless;distastefully;;;;;;;
                horrifyingly;harass;dun;;;;;;;
                horror-stricken;heartbreaking;enfuriate;;;;;;;
                horror-struck;heartrending;enraged;;;;;;;
                hysterical;heartsick;enviable;;;;;;;
                hysterically;heavyhearted;enviably;;;;;;;
                intimidate;joyless;envious;;;;;;;
                intimidated;joylessly;enviously;;;;;;;
                monstrously;lachrymose;envy;;;;;;;
                outrageous;laden;estrange;;;;;;;
                pall;lamentably;estranged;;;;;;;
                panic;long-faced;evil;;;;;;;
                panicked;lorn;exacerbate;;;;;;;
                panicky;low;exasperate;;;;;;;
                panic-stricken;low-spirited;exasperating;;;;;;;
                panic-struck;melancholic;execrate;;;;;;;
                scare;melancholy;fed_up;;;;;;;
                scare_away;miserable;foul;;;;;;;
                scare_off;miserably;frustrate;;;;;;;
                scared;misfortunate;frustrated;;;;;;;
                scarey;mournful;frustrating;;;;;;;
                scarily;mournfully;furious;;;;;;;
                scary;mourning;furiously;;;;;;;
                shivery;oppress;galling;;;;;;;
                shuddery;oppressed;get_at;;;;;;;
                shy;oppressive;get_to;;;;;;;
                shyly;oppressively;grabby;;;;;;;
                suspenseful;pathetic;grasping;;;;;;;
                suspensive;penitent;gravel;;;;;;;
                terrible;penitentially;greedy;;;;;;;
                terrified;penitently;green-eyed;;;;;;;
                timid;persecute;grizzle;;;;;;;
                timidly;persecuted;grudge;;;;;;;
                timorous;piteous;grudging;;;;;;;
                timorously;pitiable;harass;;;;;;;
                trepid;pitiful;harassed;;;;;;;
                trepidly;pitying;harried;;;;;;;
                ugly;plaintive;hate;;;;;;;
                unassertive;plaintively;hateful;;;;;;;
                unassertively;poor;hatefully;;;;;;;
                uneasily;regret;hideous;;;;;;;
                unkind;regretful;horrid;;;;;;;
                unsure;remorseful;horrific;;;;;;;
                ;remorsefully;hostile;;;;;;;
                ;repent;hostilely;;;;;;;
                ;repentant;huffily;;;;;;;
                ;repentantly;huffish;;;;;;;
                ;rue;huffy;;;;;;;
                ;rueful;incense;;;;;;;
                ;ruefully;incensed;;;;;;;
                ;sad;indignant;;;;;;;
                ;sadden;indignantly;;;;;;;
                ;saddening;indispose;;;;;;;
                ;sadly;infuriate;;;;;;;
                ;shamed;infuriated;;;;;;;
                ;shamefaced;infuriating;;;;;;;
                ;sorrow;inimical;;;;;;;
                ;sorrowful;irascible;;;;;;;
                ;sorrowfully;irritate;;;;;;;
                ;sorrowing;irritated;;;;;;;
                ;sorry;irritating;;;;;;;
                ;sorry_for;isolated;;;;;;;
                ;suffering;jealous;;;;;;;
                ;tearful;jealously;;;;;;;
                ;tyrannical;livid;;;;;;;
                ;tyrannous;lividly;;;;;;;
                ;uncheerful;loathe;;;;;;;
                ;unhappy;loathly;;;;;;;
                ;weeping;loathsome;;;;;;;
                ;woebegone;mad;;;;;;;
                ;woeful;maddened;;;;;;;
                ;woefully;maddening;;;;;;;
                ;world-weary;malefic;;;;;;;
                ;wretched;malevolent;;;;;;;
                ;;malevolently;;;;;;;
                ;;malicious;;;;;;;
                ;;maliciously;;;;;;;
                ;;malign;;;;;;;
                ;;misanthropic;;;;;;;
                ;;misanthropical;;;;;;;
                ;;misogynic;;;;;;;
                ;;murderously;;;;;;;
                ;;nark;;;;;;;
                ;;nauseate;;;;;;;
                ;;nauseated;;;;;;;
                ;;nauseating;;;;;;;
                ;;nauseous;;;;;;;
                ;;nettle;;;;;;;
                ;;nettled;;;;;;;
                ;;nettlesome;;;;;;;
                ;;noisome;;;;;;;
                ;;obscene;;;;;;;
                ;;odiously;;;;;;;
                ;;offend;;;;;;;
                ;;offensive;;;;;;;
                ;;oppress;;;;;;;
                ;;outrage;;;;;;;
                ;;outraged;;;;;;;
                ;;outrageous;;;;;;;
                ;;overjealous;;;;;;;
                ;;peeved;;;;;;;
                ;;persecute;;;;;;;
                ;;pesky;;;;;;;
                ;;pestered;;;;;;;
                ;;pestering;;;;;;;
                ;;pestiferous;;;;;;;
                ;;pique;;;;;;;
                ;;pissed;;;;;;;
                ;;plaguey;;;;;;;
                ;;plaguy;;;;;;;
                ;;pout;;;;;;;
                ;;prehensile;;;;;;;
                ;;provoked;;;;;;;
                ;;queasy;;;;;;;
                ;;rag;;;;;;;
                ;;reject;;;;;;;
                ;;repel;;;;;;;
                ;;repellant;;;;;;;
                ;;repellent;;;;;;;
                ;;repugnant;;;;;;;
                ;;repulse;;;;;;;
                ;;repulsive;;;;;;;
                ;;repulsively;;;;;;;
                ;;resentful;;;;;;;
                ;;resentfully;;;;;;;
                ;;revengefully;;;;;;;
                ;;revolt;;;;;;;
                ;;revolting;;;;;;;
                ;;revoltingly;;;;;;;
                ;;rile;;;;;;;
                ;;riled;;;;;;;
                ;;roiled;;;;;;;
                ;;scorn;;;;;;;
                ;;see_red;;;;;;;
                ;;separated;;;;;;;
                ;;set-apart;;;;;;;
                ;;sick;;;;;;;
                ;;sick_of;;;;;;;
                ;;sicken;;;;;;;
                ;;sickening;;;;;;;
                ;;sickeningly;;;;;;;
                ;;sickish;;;;;;;
                ;;sore;;;;;;;
                ;;spiteful;;;;;;;
                ;;stew;;;;;;;
                ;;stung;;;;;;;
                ;;sulk;;;;;;;
                ;;sulky;;;;;;;
                ;;tantalize;;;;;;;
                ;;teasing;;;;;;;
                ;;tired_of;;;;;;;
                ;;torment;;;;;;;
                ;;turn_off;;;;;;;
                ;;umbrageous;;;;;;;
                ;;unfriendly;;;;;;;
                ;;vengefully;;;;;;;
                ;;vex;;;;;;;
                ;;vexatious;;;;;;;
                ;;vexed;;;;;;;
                ;;vexing;;;;;;;
                ;;vile;;;;;;;
                ;;vindictive;;;;;;;
                ;;vindictively;;;;;;;
                ;;wicked;;;;;;;
                ;;with_hostility;;;;;;;
                ;;wrathful;;;;;;;
                ;;wrathfully;;;;;;;
                ;;wroth;;;;;;;
                ;;wrothful;;;;;;;
                ;;yucky;;;;;;;".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

        protected static string[] positiveEmotionWordLabels = "joy;love;enthusiasm;gratitude;self-pride;calmness;fearlessness;positive-expectation;positive-hope;positive-fear;affection;liking".Split(';');

        protected static string[] positiveEmotionWords = @"appreciated;admirable;avid;appreciatively;;allay;assure;anticipate;bucked_up;;;
            banter;admirably;eager;grateful;;assuasive;confident;cliff-hanging;encourage;;;
            barrack;admire;eagerly;gratefully;;at_ease;convinced;look_for;encouraged;;;
            be_on_cloud_nine;adorably;ebulliently;thankful;;calm;dauntlessly;look_to;encouraging;;;
            beaming;adoring;enthusiastic;;;calm_down;doughty;suspenseful;encouragingly;;;
            blithely;affect;enthusiastically;;;calming;fearless;suspensive;hope;;;
            carefree;affectional;exciting;;;calmly;fearlessly;;hopeful;;;
            chaff;affectionate;expansively;;;chill;hardy;;hopefully;;;
            cheer;affective;exuberantly;;;cold;intrepidly;;optimistic;;;
            cheer_up;amative;great;;;cool;positive;;optimistically;;;
            cheerful;amatory;riotously;;;cool_down;reassure;;sanguine;;;
            cheerfully;amicable;thirstily;;;dreamy;reassured;;;;;
            cheering;amicably;zealous;;;ease;reassuring;;;;;
            chirk_up;amorous;;;;easily;reassuringly;;;;;
            close;approbative;;;;easy;unafraid;;;;;
            comfort;approbatory;;;;lackadaisical;;;;;;
            comfortable;approve;;;;languid;;;;;;
            comfortably;approved;;;;languorous;;;;;;
            comforting;approving;;;;languorously;;;;;;
            complacent;becharm;;;;lull;;;;;;
            congratulate;beguile;;;;lulling;;;;;;
            console;beguiled;;;;pacifically;;;;;;
            content;benefic;;;;pacifying;;;;;;
            contented;beneficed;;;;peaceable;;;;;;
            ebulliently;beneficent;;;;peaceably;;;;;;
            elate;beneficially;;;;peace-loving;;;;;;
            elated;benevolent;;;;placid;;;;;;
            elating;benevolently;;;;placidly;;;;;;
            embolden;bewitch;;;;quiet;;;;;;
            euphoriant;bewitching;;;;quieten;;;;;;
            euphoric;brotherlike;;;;quietening;;;;;;
            exalt;brotherly;;;;relieve;;;;;;
            exhilarate;captivate;;;;serene;;;;;;
            exhilarated;captivated;;;;soothing;;;;;;
            exhilarating;captivating;;;;still;;;;;;
            exhort;capture;;;;tranquil;;;;;;
            expansively;caring;;;;tranquilize;;;;;;
            exuberantly;caring;;;;tranquillize;;;;;;
            exult;catch;;;;tranquilly;;;;;;
            exultant;charm;;;;unagitated;;;;;;
            exultantly;charmed;;;;unruffled;;;;;;
            exulting;commendable;;;;;;;;;;
            exultingly;delighted;;;;;;;;;;
            fulfil;devoted;;;;;;;;;;
            fulfill;emotive;;;;;;;;;;
            gay;enamor;;;;;;;;;;
            gayly;enamour;;;;;;;;;;
            glad;enchant;;;;;;;;;;
            gladden;enchanting;;;;;;;;;;
            gladdened;endearingly;;;;;;;;;;
            gladsome;enjoy;;;;;;;;;;
            gleeful;enthralled;;;;;;;;;;
            gleefully;enthralling;;;;;;;;;;
            gloatingly;entrance;;;;;;;;;;
            gratify;entranced;;;;;;;;;;
            gratifying;entrancing;;;;;;;;;;
            gratifyingly;fascinate;;;;;;;;;;
            happily;fascinating;;;;;;;;;;
            happy;favor;;hero;;;;;;;;
            hearten;favorable;;;;;;;;;;
            hilarious;favorably;;;;;;;;;;
            hilariously;favour;;;;;;;;;;
            inspire;favourable;;;;;;;;;;
            intoxicate;favourably;;;;;;;;;;
            jocund;fond;;;;;;;;;;
            jolly;fondly;;;;;;;;;;
            jolly_along;fraternal;;;;;;;;;;
            jolly_up;friendly;;;;;;;;;;
            jovial;giving_protection;;;;;;;;;;
            joy;good;;;;;;;;;;
            joyful;impress;;;;;;;;;;
            joyfully;laudably;;;;;;;;;;
            joyous;likable;;;;;;;;;;
            joyously;like;;;;;;;;;;
            jubilant;likeable;;;;;;;;;;
            jubilantly;look_up_to;;;;;;;;;;
            jubilate;love;;;;;;;;;;
            jump_for_joy;lovesome;;;;;;;;;;
            kid;loving;;;;;;;;;;
            lift_up;lovingly;;;;;;;;;;
            live_up_to;move;;;;;;;;;;
            merrily;offering_protection;;;;;;;;;;
            merry;praiseworthily;;;;;;;;;;
            mirthful;protective;;;;;;;;;;
            mirthfully;protectively;;;;;;;;;;
            near;romantic;;;;;;;;;;
            nigh;strike;;;;;;;;;;
            pep_up;tender;;;;;;;;;;
            pick_up;trance;;;;;;;;;;
            pleased;warm;;;;;;;;;;
            pleasing;warmhearted;;;;;;;;;;
            preen;worshipful;;;;;;;;;;
            pride;;;;;;;;;;;
            prideful;;;;;;;;;;;
            proudly;;;;;;;;;;;
            recreate;;;;;;;;;;;
            rejoice;;;;;;;;;;;
            rejoicing;;;;;;;;;;;
            revel;;;;;;;;;;;
            riotously;;;;;;;;;;;
            satiable;;;;;;;;;;;
            satisfactorily;;;;;;;;;;;
            satisfactory;;;;;;;;;;;
            satisfiable;;;;;;;;;;;
            satisfied;;;;;;;;;;;
            satisfy;;;;;;;;;;;
            satisfying;;;;;;;;;;;
            satisfyingly;;;;;;;;;;;
            screaming;;;;;;;;;;;
            self-satisfied;;;;;;;;;;;
            smug;;;;;;;;;;;
            solace;;;;;;;;;;;
            soothe;;;;;;;;;;;
            stimulating;;;;;;;;;;;
            teased;;;thanks;;;;;;;;
            thrill;;;;;;;;;;;
            tickle;;;;;;;;;;;
            titillate;;;;;;;;;;;
            titillated;;;;;;;;;;;
            titillating;;;;;;;;;;;
            triumph;;;;;;;;;;;
            triumphal;;;;;;;;;;;
            triumphant;;;;;;;;;;;
            triumphantly;;;;;;;;;;;
            unworried;;;;;;;;;;;
            uplift;;;;;;;;;;;
            uproarious;;;;;;;;;;;
            uproariously;;;;;;;;;;;
            urge;;;;;;;;;;;
            urge_on;;;;;;;;;;;
            walk_on_air;;;;;;;;;;;
            wallow;;;;;;;;;;;
            with_happiness;;;;;;;;;;;
            with_pride;;;;;;;;;;;".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

        protected static string[] ambiguousEmotionWordLabels = "thing; gravity;surprise;ambiguous-agitation;ambiguous-fear;pensiveness;ambiguous-expectation".Split(';');

        protected static string[] ambiguousEmotionWords = @";dear;amaze;agitate;fear;brooding;anticipant
            ;devout;amazed;agitated;hero-worship;broody;anticipate
            ;earnest;amazing;electrifying;idolize;contemplative;anticipative
            ;earnestly;amazingly;excite;revere;meditative;desire
            ;heartfelt;astonied;fire_up;reverence;musing;expect
            ;in_earnest;astonish;foment;reverent;pensive;expectant
            ;seriously;astonished;heat;reverentially;pensively;expectantly
            ;;astonishing;ignite;reverently;pondering;fevered
            ;;astonishingly;incite;venerate;reflective;feverish
            ;;astound;inflame;worship;ruminative;feverishly
            ;;astounded;instigate;;wistful;hectic
            ;;astounding;scandalmongering;;;hope
            ;;awe;sensational;;;hopeful
            ;;awed;sensationalistic;;;hopefully
            ;;awestricken;sensationally;;;trust
            ;;awestruck;set_off;;;
            ;;awful;shake;;;
            ;;baffle;shake_up;;;
            ;;beat;stimulate;;;
            ;;besot;stir;;;
            ;;bewilder;stir_up;;;
            ;;dazed;thrilling;;;
            ;;dumbfound;touch;;;
            ;;dumbfounded;yellow;;;
            ;;dumfounded;;;;
            ;;fantastic;;;;
            ;;flabbergasted;;;;
            ;;flummox;;;;
            ;;get;;;;
            ;;gravel;;;;
            ;;howling;;;;
            ;;in_awe_of;;;;
            ;;marvel;;;;
            ;;marvellously;;;;
            ;;marvelous;;;;
            ;;marvelously;;;;
            ;;mystify;;;;
            ;;nonplus;;;;
            ;;perplex;;;;
            ;;puzzle;;;;
            ;;rattling;;;;
            ;;staggering;;;;
            ;;stun;;;;
            ;;stunned;;;;
            ;;stupefied;;;;
            ;;stupefy;;;;
            ;;stupefying;;;;
            ;;stupid;;;;
            ;;stupify;;;;
            ;;superbly;;;;
            ;;surprise;;;;
            ;;surprised;;;;
            ;;surprisedly;;;;
            ;;surprising;;;;
            ;;surprisingly;;;;
            ;;terrific;;;;
            ;;terrifically;;;;
            ;;thunderstruck;;;;
            ;;toppingly;;;;
            ;;tremendous;;;;
            ;;trounce;;;;
            ;;wonder;;;;
            ;;wonderful;;;;
            ;;wonderfully;;;;
            ;;wondrous;;;;
            ;;wondrously;;;;".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

        protected static string[] emoticonWords = @"%-(	-1
            %-)	1
            (-:	1
            (:	1
            (^ ^)	1
            (^-^)	1
            (^.^)	1
            (^_^)	1
            (o:	1
            (o;	0
            )-:	-1
            ):	-1
            )o:	-1
            *)	0
            *\o/*	1
            --^--@	1
            0:)	1
            38*	-1
            8)	1
            8-)	0
            8-0	-1
            8/	-1
            8\	-1
            8c	-1
            :#	-1
            :'(	-1
            :'-(	-1
            :(	-1
            :)	1
            :*(	-1
            :,(	-1
            :-&	-1
            :-(	-1
            :-(o)	-1
            :-)	1
            :-*	1
            :-*	1
            :-/	-1
            :-/	0
            :-D	1
            :-O	0
            :-P	1
            :-S	-1
            :-\	-1
            :-\	0
            :-|	-1
            :-}	1
            :/	-1
            :0->-<|:	0
            :3	1
            :9	1
            :D	1
            :E	-1
            :F	-1
            :O	-1
            :P	1
            :P	1
            :S	-1
            :X	1
            :[	-1
            :[	-1
            :\	-1
            :]	1
            :_(	-1
            :b)	1
            :l	0
            :o(	-1
            :o)	1
            :p	1
            :s	-1
            :|	-1
            :|	0
            :Þ	1
            :…(	-1
            ;)	0
            ;^)	1
            ;o)	0
            </3-1	-1
            <3	1
            <:}	0
            <o<	-1
            =(	-1
            =)	1
            =[	-1
            =]	1
            >/	-1
            >:(	-1
            >:)	1
            >:D	1
            >:L	-1
            >:O	-1
            >=D	1
            >[	-1
            >\	-1
            >o>	-1
            @}->--	1
            B(	-1
            Bc	-1
            D:	-1
            X(	-1
            X(	-1
            X-(	-1
            XD	1
            XD	1
            XO	-1
            XP	-1
            XP	1
            ^_^	1
            ^o)	-1
            x3?	1
            xD	1
            xP	-1
            |8C	-1
            |8c	-1
            |D	1
            }:)	1".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

        protected static string[] identityHateWords = {
            "uncivilised",
            "gypo",
            "gypos",
            "cunt",
            "cunts",
            "peckerwood",
            "peckerwoods",
            "raghead",
            "ragheads",
            "cripple",
            "cripples",
            "niggur",
            "niggurs",
            "yellow bone",
            "yellow bones",
            "muzzie",
            "muzzies",
            "niggar",
            "niggars",
            "nigger",
            "niggers",
            "greaseball",
            "greaseballs",
            "white trash",
            "white trashes",
            "nig nog",
            "nig nogs",
            "faggot",
            "faggots",
            "cotton picker",
            "cotton pickers",
            "darkie",
            "darkies",
            "hoser",
            "hosers",
            "Uncle Tom",
            "Uncle Toms",
            "Jihadi",
            "Jihadis",
            "retard",
            "retards",
            "hillbilly",
            "hillbillies",
            "fag",
            "fags",
            "trailer trash",
            "trailer trashes",
            "pikey",
            "pikies",
            "nicca",
            "niccas",
            "tranny",
            "trannies",
            "porch monkey",
            "porch monkies",
            "wigger",
            "wiggers",
            "wetback",
            "wetbacks",
            "nigglet",
            "nigglets",
            "wigga",
            "wiggas",
            "dhimmi",
            "dhimmis",
            "honkey",
            "honkies",
            "eurotrash",
            "eurotrashes",
            "yardie",
            "yardies",
            "trailer park trash",
            "trailer park trashes",
            "niggah",
            "niggahes",
            "yokel",
            "yokels",
            "nigguh",
            "nigguhes",
            "camel jockey",
            "camel jockies",
            "honkie",
            "honkies",
            "niglet",
            "niglets",
            "gyppo",
            "gyppos",
            "dyke",
            "dykes",
            "half breed",
            "honky",
            "honkies",
            "race traitor",
            "race traitors",
            "jiggaboo",
            "jiggaboos",
            "Chinaman",
            "Chinamans",
            "curry muncher",
            "curry munchers",
            "jungle bunny",
            "jungle bunnies",
            "coon ass",
            "coon asses",
            "newfie",
            "newfies",
            "house nigger",
            "house niggers",
            "limey",
            "limies",
            "red bone",
            "red bones",
            "guala",
            "gualas",
            "plastic paddy",
            "plastic paddies",
            "whigger",
            "whiggers",
            "jigaboo",
            "jigaboos",
            "nig",
            "nigs",
            "Zionazi",
            "Zionazis",
            "spear chucker",
            "spear chuckers",
            "niggress",
            "niggresses",
            "yobbo",
            "yobbos",
            "border jumper",
            "border jumpers",
            "sperg",
            "spergs",
            "pommy",
            "pommies",
            "munter",
            "munters",
            "tar baby",
            "tar babies",
            "pommie",
            "pommies",
            "gyp",
            "gyps",
            "anchor baby",
            "anchor babies",
            "twat",
            "twats",
            "border hopper",
            "border hoppers",
            "queer",
            "queers",
            "darky",
            "darkies",
            "ching chong",
            "ching chongs",
            "khazar",
            "khazars",
            "gippo",
            "gippos",
            "skanger",
            "skangers",
            "beaner",
            "beaners",
            "quadroon",
            "quadroons",
            "gator bait",
            "gator baits",
            "Cushite",
            "Cushites",
            "mud shark",
            "mud sharks",
            "cracker",
            "crackers",
            "dune coon",
            "dune coons",
            "pickaninny",
            "pickaninnies",
            "slant eye",
            "slant eyes",
            "sideways vagina",
            "sideways vaginas",
            "hick",
            "hicks",
            "camel fucker",
            "camel fuckers",
            "redneck",
            "rednecks",
            "spiv",
            "spivs",
            "zipperhead",
            "zipperheads",
            "Kushite",
            "Kushites",
            "Shylock",
            "Shylocks",
            "gook",
            "gooks",
            "papist",
            "papists",
            "hymie",
            "hymies",
            "wog",
            "wogs",
            "scally",
            "scallies",
            "coon",
            "coons",
            "whitey",
            "whities",
            "nigette",
            "nigettes",
            "paki",
            "pakis",
            "towel head",
            "towel heads",
            "Argie",
            "Argies",
            "wexican",
            "wexicans",
            "jigger",
            "jiggers",
            "injun",
            "injuns",
            "ocker",
            "ockers",
            "polack",
            "polacks",
            "moulie",
            "moulies",
            "niggor",
            "niggors",
            "scanger",
            "scangers",
            "ofay",
            "ofaies",
            "jigga",
            "jiggas",
            "redskin",
            "redskins",
            "chonky",
            "chonkies",
            "hebro",
            "hebros",
            "wop",
            "wops",
            "chink",
            "chinks",
            "sideways pussy",
            "sideways pussies",
            "paleface",
            "palefaces",
            "wagon burner",
            "wagon burners",
            "nigra",
            "nigras",
            "spic",
            "spics",
            "spics",
            "jocky",
            "jockies",
            "kraut",
            "krauts",
            "steek",
            "steeks",
            "coolie",
            "coolies",
            "gooky",
            "gookies",
            "octaroon",
            "octaroons",
            "bint",
            "bints",
            "shit heel",
            "shit heels",
            "squaw",
            "squaws",
            "bog trotter",
            "bog trotters",
            "Oriental",
            "Orientals",
            "halfrican",
            "halfricans",
            "paddy",
            "paddies",
            "groid",
            "groids",
            "jiggabo",
            "jiggabos",
            "jigg",
            "jiggs",
            "jant",
            "jants",
            "spide",
            "spides",
            "camel humper",
            "camel humpers",
            "white nigger",
            "white niggers",
            "ZOG",
            "ZOGs",
            "diaper head",
            "diaper heads",
            "heeb",
            "heebs",
            "Christ killer",
            "Christ killers",
            "piker",
            "pikers",
            "higger",
            "higgers",
            "lemonhead",
            "lemonheads",
            "Hun",
            "Huns",
            "popolo",
            "popolos",
            "cowboy killer",
            "cowboy killers",
            "jhant",
            "jhants",
            "eyetie",
            "eyeties",
            "mockey",
            "mockies",
            "alligator bait",
            "alligator baits",
            "Jap",
            "Japs",
            "shanty Irish",
            "shanty Irishes",
            "redlegs",
            "mulignan",
            "mulignans",
            "jockie",
            "jockies",
            "mangia cake",
            "mangia cakes",
            "moulinyan",
            "moulinyans",
            "nigar",
            "nigars",
            "darkey",
            "darkies",
            "gurrier",
            "gurriers",
            "lubra",
            "lubras",
            "Buckwheat",
            "Buckwheats",
            "mulato",
            "mulatos",
            "prairie nigger",
            "prairie niggers",
            "kyke",
            "kykes",
            "boonie",
            "boonies",
            "mick",
            "micks",
            "bluegum",
            "bluegums",
            "spigger",
            "spiggers",
            "border bunny",
            "border bunnies",
            "kike",
            "kikes",
            "moulignon",
            "moulignons",
            "roundeye",
            "roundeyes",
            "ginzo",
            "ginzos",
            "Jewbacca",
            "Jewbaccas",
            "booner",
            "booners",
            "nigre",
            "nigres",
            "scallie",
            "scallies",
            "niger",
            "nigers",
            "dinge",
            "dinges",
            "Leb",
            "Lebs",
            "Lebbo",
            "Lebbos",
            "sambo",
            "sambos",
            "Africoon",
            "Africoons",
            "ling ling",
            "ling lings",
            "gub",
            "gubs",
            "banana bender",
            "banana benders",
            "japie",
            "japies",
            "island nigger",
            "island niggers",
            "hairyback",
            "hairybacks",
            "lugan",
            "lugans",
            "Bog Irish",
            "Bog Irishes",
            "blaxican",
            "blaxicans",
            "moke",
            "mokes",
            "nigor",
            "nigors",
            "bix nood",
            "bix noods",
            "Kushi",
            "Kushis",
            "guala guala",
            "guala gualas",
            "hoosier",
            "hoosiers",
            "thicklips",
            "mook",
            "mooks",
            "muk",
            "muks",
            "soup taker",
            "soup takers",
            "senga",
            "sengas",
            "Cushi",
            "Cushis",
            "pogue",
            "pogues",
            "abo",
            "abos",
            "ping pang",
            "ping pangs",
            "proddy dog",
            "proddy dogs",
            "boong",
            "boongs",
            "dago",
            "dagos",
            "dogun",
            "doguns",
            "mocky",
            "mockies",
            "poppadom",
            "poppadoms",
            "Gwat",
            "Gwats",
            "ice nigger",
            "ice niggers",
            "spook",
            "spooks",
            "Afro-Saxon",
            "Afro-Saxons",
            "guido",
            "guidos",
            "latrino",
            "latrinos",
            "lowlander",
            "lowlanders",
            "mockie",
            "mockies",
            "moky",
            "mokies",
            "mosshead",
            "mossheads",
            "African catfish",
            "African catfishes",
            "gyppy",
            "gyppies",
            "timber nigger",
            "timber niggers",
            "Americoon",
            "Americoons",
            "camel cowboy",
            "camel cowboies",
            "eh hole",
            "eh holes",
            "Hunyak",
            "Hunyaks",
            "slopehead",
            "slopeheads",
            "teabagger",
            "teabaggers",
            "Armo",
            "Armos",
            "bitch",
            "bitches",
            "greaser",
            "greasers",
            "Honyock",
            "Honyocks",
            "mud person",
            "mud persons",
            "pineapple nigger",
            "pineapple niggers",
            "retarded",
            "semihole",
            "semiholes",
            "Amo",
            "Amos",
            "border nigger",
            "border niggers",
            "buckra",
            "buckras",
            "burrhead",
            "burrheads",
            "cab nigger",
            "cab niggers",
            "carpet pilot",
            "carpet pilots",
            "pancake face",
            "pancake faces",
            "spigotty",
            "spigotties",
            "carrot snapper",
            "carrot snappers",
            "chili shitter",
            "chili shitters",
            "curry slurper",
            "curry slurpers",
            "ghetto hamster",
            "ghetto hamsters",
            "ice monkey",
            "ice monkies",
            "roofucker",
            "roofuckers",
            "Velcro head",
            "Velcro heads",
            "wiggerette",
            "wiggerettes",
            "beach nigger",
            "beach niggers",
            "bean dipper",
            "bean dippers",
            "bog hopper",
            "bog hoppers",
            "Buddhahead",
            "Buddhaheads",
            "camel jacker",
            "camel jackers",
            "Caublasian",
            "Caublasians",
            "cave nigger",
            "cave niggers",
            "cow kisser",
            "cow kissers",
            "dune nigger",
            "dune niggers",
            "four by two",
            "four by twos",
            "fresh off the boat",
            "fresh off the boats",
            "gin jockey",
            "gin jockies",
            "golliwog",
            "golliwogs",
            "guinea",
            "guineas",
            "Jim Fish",
            "Jim Fishes",
            "mackerel snapper",
            "mackerel snappers",
            "octroon",
            "octroons",
            "pohm",
            "pohms",
            "pussy",
            "pussies",
            "Russellite",
            "Russellites",
            "spice nigger",
            "spice niggers",
            "uncivilized",
            "Whipped",
            "albino",
            "albinos",
            "ape",
            "apes",
            "Aunt Jemima",
            "Aunt Jemimas",
            "buckethead",
            "bucketheads",
            "Chinese wetback",
            "Chinese wetbacks",
            "chug",
            "chugs",
            "curry stinker",
            "curry stinkers",
            "dyke jumper",
            "dyke jumpers",
            "eight ball",
            "eight balls",
            "gun burglar",
            "gun burglars",
            "ikey mo",
            "ikey mos",
            "lawn jockey",
            "lawn jockies",
            "leprechaun",
            "leprechauns",
            "mutt",
            "mutts",
            "negro",
            "negros",
            "negroes",
            "nitchee",
            "nitchees",
            "sooty",
            "sooties",
            "spick",
            "spicks",
            "tinkard",
            "tinkards",
            "uncircumcised baboon",
            "uncircumcised baboons",
            "zigabo",
            "zigabos",
            "abbo",
            "abbos",
            "Anglo",
            "Anglos",
            "Aunt Jane",
            "Aunt Janes",
            "Aunt Mary",
            "Aunt Maries",
            "Aunt Sally",
            "Aunt Sallies",
            "azn",
            "azns",
            "bamboo coon",
            "bamboo coons",
            "banana lander",
            "banana landers",
            "banjo lips",
            "bans and cans",
            "beaner shnitzel",
            "beaner shnitzels",
            "beaney",
            "beanies",
            "Bengali",
            "Bengalis",
            "bhrempti",
            "bhremptis",
            "bird",
            "birds",
            "bitter clinger",
            "bitter clingers",
            "black Barbie",
            "black Barbies",
            "black dago",
            "black dagos",
            "blockhead",
            "blockheads",
            "bog jumper",
            "bog jumpers",
            "boon",
            "boons",
            "boonga",
            "boongas",
            "Bounty bar",
            "Bounty bars",
            "boxhead",
            "boxheads",
            "brass ankle",
            "brass ankles",
            "brownie",
            "brownies",
            "buffie",
            "buffies",
            "bug eater",
            "bug eaters",
            "buk buk",
            "buk buks",
            "bumblebee",
            "bumblebees",
            "bung",
            "bungs",
            "bunga",
            "bungas",
            "butterhead",
            "butterheads",
            "can eater",
            "can eaters",
            "celestial",
            "celestials",
            "Charlie",
            "Charlies",
            "chee chee",
            "chee chees",
            "chi chi",
            "chi chis",
            "chigger",
            "chiggers",
            "chinig",
            "chinigs",
            "chink a billy",
            "chink a billies",
            "chunky",
            "chunkies",
            "clam",
            "clams",
            "clamhead",
            "clamheads",
            "colored",
            "coloured",
            "crow",
            "crows",
            "dego",
            "degos",
            "dink",
            "dinks",
            "dogan",
            "dogans",
            "domes",
            "dot head",
            "dot heads",
            "eggplant",
            "eggplants",
            "Fairy",
            "Fairies",
            "fez",
            "fezs",
            "FOB",
            "FOBs",
            "fog nigger",
            "fog niggers",
            "fuzzy",
            "fuzzies",
            "fuzzy wuzzy",
            "fuzzy wuzzies",
            "gable",
            "gables",
            "Gerudo",
            "Gerudos",
            "gew",
            "gews",
            "ghetto",
            "ghettos",
            "gipp",
            "gipps",
            "gook eye",
            "gook eyes",
            "gyppie",
            "gyppies",
            "heinie",
            "heinies",
            "ho",
            "hos",
            "hoe",
            "hoes",
            "Honyak",
            "Honyaks",
            "Hunkie",
            "Hunkies",
            "Hunky",
            "Hunkies",
            "Hunyock",
            "Hunyocks",
            "ike",
            "ikes",
            "ikey",
            "ikies",
            "iky",
            "ikies",
            "jig",
            "jigs",
            "jigarooni",
            "jigaroonis",
            "jijjiboo",
            "jijjiboos",
            "kotiya",
            "kotiyas",
            "mickey",
            "mickies",
            "moch",
            "moches",
            "mock",
            "mocks",
            "mong",
            "mongs",
            "monkey",
            "monkies",
            "Moor",
            "Moors",
            "moss eater",
            "moss eaters",
            "moxy",
            "moxies",
            "muktuk",
            "muktuks",
            "mung",
            "mungs",
            "munt",
            "munts",
            "ned",
            "net head",
            "net heads",
            "nichi",
            "nichis",
            "nichiwa",
            "nichiwas",
            "nidge",
            "nidges",
            "nip",
            "nips",
            "nitchie",
            "nitchies",
            "nitchy",
            "nitchies",
            "Orangie",
            "Orangies",
            "Oreo",
            "Oreos",
            "papoose",
            "papooses",
            "piky",
            "pikies",
            "pinto",
            "pintos",
            "pointy head",
            "pointy heads",
            "pollo",
            "pollos",
            "pom",
            "poms",
            "pommie grant",
            "pommie grants",
            "Punjab",
            "Punjabs",
            "rube",
            "rubes",
            "sawney",
            "sawnies",
            "scag",
            "scags",
            "seppo",
            "seppos",
            "septic",
            "septics",
            "shant",
            "shants",
            "sheeny",
            "sheenies",
            "sheepfucker",
            "sheepfuckers",
            "Shelta",
            "Sheltas",
            "shiner",
            "shiners",
            "shit kicker",
            "shit kickers",
            "Shy",
            "Shies",
            "sideways cooter",
            "sideways cooters",
            "skag",
            "skags",
            "Skippy",
            "Skippies",
            "slag",
            "slags",
            "slant",
            "slants",
            "slit",
            "slits",
            "slope",
            "slopes",
            "slopey",
            "slopies",
            "slopy",
            "slopies",
            "smoke jumper",
            "smoke jumpers",
            "smoked Irish",
            "smoked Irishes",
            "smoked Irishman",
            "smoked Irishmans",
            "sole",
            "soles",
            "spickaboo",
            "spickaboos",
            "spig",
            "spigs",
            "spik",
            "spiks",
            "spink",
            "spinks",
            "squarehead",
            "squareheads",
            "squinty",
            "squinties",
            "stovepipe",
            "stovepipes",
            "sub human",
            "sub humans",
            "sucker fish",
            "sucker fishes",
            "Taffy",
            "Taffies",
            "teapot",
            "teapots",
            "tenker",
            "tenkers",
            "tincker",
            "tinckers",
            "tinkar",
            "tinkars",
            "tinker",
            "tinkers",
            "tinkere",
            "tinkeres",
            "trash",
            "trashes",
            "tree jumper",
            "tree jumpers",
            "tunnel digger",
            "tunnel diggers",
            "Twinkie",
            "Twinkies",
            "tyncar",
            "tyncars",
            "tynekere",
            "tynekeres",
            "tynkard",
            "tynkards",
            "tynkare",
            "tynkares",
            "tynker",
            "tynkers",
            "tynkere",
            "tynkeres",
            "WASP",
            "WASPs",
            "Yank",
            "Yanks",
            "Yankee",
            "Yankees",
            "yellow",
            "yellows",
            "yid",
            "yids",
            "yob",
            "yobs",
            "zebra",
            "zebras",
            "zippohead",
            "zippoheads",
            "ZOG lover",
            "ZOG lovers",
            "knacker",
            "knackers",
            "shyster",
            "shysters",
            "bogan",
            "bogans",
            "hayseed",
            "moon cricket",
            "moon crickets",
            "mud duck",
            "mud ducks",
            "surrender monkey",
            "surrender monkies",
            "bludger",
            "bludgers",
            "charver",
            "charvers",
            "dole bludger",
            "dole bludgers",
            "chav",
            "chavs",
            "sheister",
            "sheisters",
            "charva",
            "charvas",
            "touch of the tar brush",
            "touch of the tar brushes",
            "Northern monkey",
            "Northern monkies",
            "Southern fairy",
            "Southern fairies",
            "gubba",
            "gubbas",
            "stump jumper",
            "stump jumpers",
            "hebe",
            "hebes",
            "millie",
            "millies",
            "quashie",
            "quashies",
            "dingo fucker",
            "dingo fuckers",
            "mil bag",
            "mil bags",
            "conspiracy theorist",
            "conspiracy theorists",
            "whore from Fife",
            "whore from Fifes",
            "boojie",
            "boojies",
            "book book",
            "book books",
            "cheese eating surrender monkey",
            "cheese eating surrender monkies",
            "idiot",
            "idiots",
            "jock",
            "jocks",
            "mack",
            "macks",
            "Merkin",
            "Merkins",
            "neche",
            "neches",
            "neejee",
            "neejees",
            "neechee",
            "neechees",
            "powderburn",
            "powderburns",
            "proddywhoddy",
            "proddywhoddies",
            "proddywoddy",
            "proddywoddies",
            "Rhine monkey",
            "Rhine monkies" };

        protected static string[] hatePhrases =
            @"allah akbar,0.87
            blacks,0.583
            chink,0.467
            chinks,0.542
            dykes,0.602
            faggot,0.489
            faggots,0.675
            fags,0.543
            homo,0.667
            inbred,0.583
            nigger,0.584
            niggers,0.672
            queers,0.5
            raped,0.717
            savages,0.778
            slave,0.667
            spic,0.75
            wetback,0.667
            wetbacks,0.688
            whites,0.556
            a dirty,0.743
            a nigger,0.509
            all niggers,0.859
            all white,0.75
            always fuck,0.556
            ass white,0.542
            be killed,0.571
            beat him,0.575
            biggest faggot,0.667
            blame the,0.533
            butt ugly,0.5
            chink eyed,0.667
            chinks in,0.556
            coon shit,0.667
            dumb monkey,0.667
            dumb nigger,0.667
            fag and,0.462
            fag but,0.556
            faggot a,0.667
            faggot and,0.556
            faggot ass,0.771
            faggot bitch,0.667
            faggot for,0.533
            faggot smh,0.5
            faggot that,0.583
            faggots and,0.667
            faggots like,0.901
            faggots usually,0.778
            faggots who,0.5
            fags are,0.667
            fuckin faggot,0.667
            fucking faggot,0.636
            fucking gay,0.5
            fucking hate,0.685
            fucking nigger,0.8
            fucking queer,0.778
            gay ass,0.481
            get raped,0.788
            hate all,0.556
            hate faggots,0.912
            hate fat,0.667
            hate you,0.663
            here faggot,0.571
            is white,0.667
            jungle bunny,0.667
            kill all,0.667
            kill yourself,0.667
            little faggot,0.667
            many niggers,0.708
            married to,0.533
            me faggot,0.5
            my coon,0.6
            nigga ask,0.556
            niggas like,0.556
            nigger ass,0.667
            nigger is,0.6
            nigger music,0.75
            niggers are,0.633
            of fags,0.69
            of white,0.588
            raped and,0.556
            raped by,0.763
            sand nigger,0.667
            savages that,0.667
            shorty bitch,0.583
            spear chucker,0.667
            spic cop,0.778
            stupid nigger,0.667
            that fag,0.5
            that faggot,0.542
            that nigger,0.6
            the faggots,0.667
            the female,0.642
            the niggers,0.778
            their heads,0.6
            them white,0.5
            then faggot,0.5
            this nigger,0.889
            to rape,0.667
            trailer park,0.5
            trash with,0.667
            u fuckin,0.5
            ugly dyke,0.667
            up nigger,0.77
            white ass,0.556
            white boy,0.556
            white person,0.583
            white trash,0.507
            with niggas,0.5
            you fag,0.611
            you nigger,0.827
            you niggers,0.755
            your faggot,0.667
            your nigger,0.556
            a bitch made,0.533
            a fag and,0.6
            a fag but,0.556
            a faggot and,0.583
            a faggot for,0.583
            a fucking queer,0.778
            a nigga ask,0.556
            a white person,0.778
            a white trash,0.6
            all these fucking,0.667
            are all white,0.75
            be killed for,0.571
            bitch made nigga,0.533
            faggots like you,0.905
            faggots usually have,0.778
            fuck outta here,0.778
            fuck u talking,0.667
            fuck you too,0.556
            fucking hate you,0.725
            full of white,0.792
            him a nigga,0.556
            his shorty bitch,0.667
            how many niggers,0.75
            is a fag,0.75
            is a faggot,0.476
            is a fuckin,0.667
            is a fucking,0.799
            is a nigger,0.556
            like a faggot,0.632
            like da colored,0.556
            many niggers are,0.714
            nigga and his,0.556
            niggers are in,0.714
            of white trash,0.6
            shut up nigger,0.785
            still a faggot,0.583
            the biggest faggot,0.556
            the faggots who,0.556
            the fuck do,0.556
            they all look,0.778
            what a fag,0.556
            white bitch in,0.556
            white trash and,0.667
            you a fag,0.778
            you a lame,0.467
            you a nigger,0.467
            you fuck wit,0.556
            you fucking faggot,0.583
            your a cunt,0.667
            your a dirty,0.87
            your bitch in,0.467
            a bitch made nigga,0.533
            a lame nigga you,0.556
            faggot if you ever,0.467
            full of white trash,0.867
            how many niggers are,0.75
            is full of white,0.792
            lame nigga you a,0.556
            many niggers are in,0.714
            nigga you a lame,0.556
            niggers are in my,0.714
            wit a lame nigga,0.556
            you a lame bitch,0.556
            you fuck wit a,0.556".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

        protected static string[] sentimentWords = @"$:	-1.5
            %)	-0.4
            %-)	-1.5
            &-:	-0.4
            &:	-0.7
            ((-:	2.1
            (*	1.1
            (-%	-0.7
            (-*	1.3
            (-:	1.6
            (-:0	2.8
            (-:<	-0.4
            (-:o	1.5
            (-:O	1.5
            (-:{	-0.1
            (-:|>*	1.9
            (-;	1.3
            (-;|	2.1
            (8	2.6
            (:	2.2
            (:0	2.4
            (:<	-0.2
            (:o	2.5
            (:O	2.5
            (;	1.1
            (;<	0.3
            (=	2.2
            (?:	2.1
            (^:	1.5
            (^;	1.5
            (^;0	2
            (^;o	1.9
            (o:	1.6
            )':	-2
            )-':	-2.1
            )-:	-2.1
            )-:<	-2.2
            )-:{	-2.1
            ):	-1.8
            ):<	-1.9
            ):{	-2.3
            );<	-2.6
            *)	0.6
            *-)	0.3
            *-:	2.1
            *-;	2.4
            *:	1.9
            *<|:-)	1.6
            *\0/*	2.3
            *^:	1.6
            --<--<@	2.2
            .-:	-1.2
            ..###-:	-1.7
            ..###:	-1.9
            /-:	-1.3
            /:	-1.3
            /:<	-1.4
            /=	-0.9
            /^:	-1
            /o:	-1.4
            0-8	0.1
            0-|	-1.2
            0:)	1.9
            0:-)	1.4
            0:-3	1.5
            0:03	1.9
            0;^)	1.6
            0_o	-0.3
            10q	2.1
            1337	2.1
            143	3.2
            1432	2.6
            14aa41	2.4
            182	-2.9
            187	-3.1
            2g2b4g	2.8
            2g2bt	-0.1
            2qt	2.1
            3:( -2.2
            3:)	0.5
            3:-(    -2.3
            3:-)	-1.4
            4col	-2.2
            4q	-3.1
            5fs	1.5
            8)	1.9
            8-d	1.7
            8-o	-0.3
            86	-1.6
            8d	2.9
            :###..	-2.4
            :$	-0.2
            :&	-0.6
            :'(	-2.2
            :')	2.3
            :'-(	-2.4
            :'-)	2.7
            :(  -1.9
            :)	2
            :*	2.5
            :-###..	-2.5
            :-&	-0.5
            :-( -1.5
            :-)	1.3
            :-))	2.8
            :-*	1.7
            :-.	-0.9
            :-/	-1.2
            :-<	-1.5
            :-d	2.3
            :-D	2.3
            :-o	0.1
            :-p	1.2
            :-[ -1.6
            :-\	-0.9
            :-c	-1.3
            :-p	1.5
            :-|	-0.7
            :-||	-2.5
            :-Þ	0.9
            :/	-1.4
            :3	2.3
            :<	-2.1
            :>	2.1
            :?)	1.3
            :?c	-1.6
            :@	-2.5
            :d	2.3
            :D	2.3
            :l	-1.7
            :o	-0.4
            :p	1
            :s	-1.2
            :[  -2
            :\	-1.3
            :]	2.2
            :^)	2.1
            :^*	2.6
            :^/	-1.2
            :^\	-1
            :^|	-1
            :c	-2.1
            :c)	2
            :o)	2.1
            :o/	-1.4
            :o\	-1.1
            :o|	-0.6
            :P	1.4
            :{	-1.9
            :|	-0.4
            :}	2.1
            :Þ	1.1
            ;)	0.9
            ;-)	1
            ;-*	2.2
            ;-]	0.7
            ;d	0.8
            ;D	0.8
            ;]	0.6
            ;^)	1.4
            </3	-3
            <3	1.9
            <:	2.1
            <:-|	-1.4
            =)	2.2
            -3	2
            #NAME?	2.4
            #NAME?	2.4
            =/	-1.4
            3	2.1
            #NAME?	2.3
            #NAME?	2.3
            #NAME?	-1.2
            #NAME?	-1.2
            =]	1.6
            #NAME?	1.3
            =|	-0.8
            >-:	-2
            >.<	-1.3
            >:	-2.1
            >:( -2.7
            >:)	0.4
            >:-(    -2.7
            >:-)	-0.4
            >:/	-1.6
            >:o	-1.2
            >:p	1
            >:[ -2.1
            >:\	-1.7
            >;( -2.9
            >;)	0.1
            >_>^	2.1
            @:	-2.1
            @>-->--	2.1
            @}-;-'---	2.2
            aas	2.5
            aayf	2.7
            afu	-2.9
            alol	2.8
            ambw	2.9
            aml	3.4
            atab	-1.9
            awol	-1.3
            ayc	0.2
            ayor	-1.2
            Aug-00	0.3
            bfd	-2.7
            bfe	-2.6
            bff	2.9
            bffn	1
            bl	2.3
            bsod	-2.2
            btd	-2.1
            btdt	-0.1
            bz	0.4
            b^d	2.6
            cwot	-2.3
            d-':	-2.5
            d8	-3.2
            d:	-2.9
            d:<	-3.2
            d;	-2.9
            d=	-3
            doa	-2.3
            dx	-3
            ez	1.5
            fav	2.4
            fcol	-1.8
            ff	1.8
            ffs	-2.8
            fkm	-2.4
            foaf	1.8
            ftw	2
            fu	-3.7
            fubar	-3
            fwb	2.5
            fyi	0.8
            fysa	0.4
            g1	1.4
            gg	1.2
            gga	1.7
            gigo	-0.6
            gj	2
            gl	1.3
            gla	2.5
            gn	1.2
            gr8	2.7
            grrr	-0.4
            gt	1.1
            h&k	2.3
            hagd	2.2
            hagn	2.2
            hago	1.2
            hak	1.9
            hand	2.2
            hho1/2k	1.4
            hhoj	2
            hhok	0.9
            hugz	2
            hi5	1.9
            idk	-0.4
            ijs	0.7
            ilu	3.4
            iluaaf	2.7
            ily	3.4
            ily2	2.6
            iou	0.7
            iyq	2.3
            j/j	2
            j/k	1.6
            j/p	1.4
            j/t	-0.2
            j/w	1
            j4f	1.4
            j4g	1.7
            jho	0.8
            jhomf	1
            jj	1
            jk	0.9
            jp	0.8
            jt	0.9
            jw	1.6
            jealz	-1.2
            k4y	2.3
            kfy	2.3
            kia	-3.2
            kk	1.5
            kmuf	2.2
            l	2
            l&r	2.2
            laoj	1.3
            lmao	2
            lmbao	1.8
            lmfao	2.5
            lmso	2.7
            lol	2.9
            lolz	2.7
            lts	1.6
            ly	2.6
            ly4e	2.7
            lya	3.3
            lyb	3
            lyl	3.1
            lylab	2.7
            lylas	2.6
            lylb	1.6
            m8	1.4
            mia	-1.2
            mml	2
            mofo	-2.4
            muah	2.8
            mubar	-1
            musm	0.9
            mwah	2.5
            n1	1.9
            nbd	1.3
            nbif	-0.5
            nfc	-2.7
            nfw	-2.4
            nh	2.2
            nimby	-0.8
            nimjd	-0.7
            nimq	-0.2
            nimy	-1.4
            nitl	-1.5
            nme	-2.1
            noyb	-0.7
            np	1.4
            ntmu	1.4
            o-8	-0.5
            o-:	-0.3
            o-|	-1.1
            o.o	-0.6
            O.o	-0.6
            o.O	-0.6
            o:	-0.2
            o:)	1.5
            o:-)	2
            o:-3	2.2
            o:3	2.3
            o:<	-0.3
            o;^)	1.6
            ok	1.6
            o_o	-0.5
            O_o	-0.5
            o_O	-0.5
            pita	-2.4
            pls	0.3
            plz	0.3
            pmbi	0.8
            pmfji	0.3
            pmji	0.7
            po	-2.6
            ptl	2.6
            pu	-1.1
            qq	-2.2
            qt	1.8
            r&r	2.4
            rofl	2.7
            roflmao	2.5
            rotfl	2.6
            rotflmao	2.8
            rotflmfao	2.5
            rotflol	3
            rotgl	2.9
            rotglmao	1.8
            s:	-1.1
            sapfu	-1.1
            sete	2.8
            sfete	2.7
            sgtm	2.4
            slap	0.6
            slaw	2.1
            smh	-1.3
            snafu	-2.5
            sob	-2.8
            swak	2.3
            tgif	2.3
            thks	1.4
            thx	1.5
            tia	2.3
            tmi	-0.3
            tnx	1.1
            TRUE	1.8
            tx	1.5
            txs	1.1
            ty	1.6
            tyvm	2.5
            urw	1.9
            vbg	2.1
            vbs	3.1
            vip	2.3
            vwd	2.6
            vwp	2.1
            wag	-0.2
            wd	2.7
            wilco	0.9
            wp	1
            wtf	-2.8
            wtg	2.1
            wth	-2.4
            x-d	2.7
            x-p	1.8
            xd	2.7
            xlnt	3
            xoxo	3
            xoxozzz	2.3
            xp	1.2
            xqzt	1.6
            xtc	0.8
            yolo	1.1
            yoyo	0.4
            yvw	1.6
            yw	1.8
            ywia	2.5
            zzz	-1.2
            [-;	0.5
            [:	1.3
            [;	1
            [=	1.7
            \-:	-1
            \:	-1
            \:<	-1.7
            \=	-1.1
            \^:	-1.3
            \o/	2.2
            \o:	-1.2
            ]-:	-2.1
            ]:	-1.6
            ]:<	-2.5
            ^<_<    1.4
            ^urs	-2.8
            abandon	-1.9
            abandoned	-2
            abandoner	-1.9
            abandoners	-1.9
            abandoning	-1.6
            abandonment	-2.4
            abandonments	-1.7
            abandons	-1.3
            abducted	-2.3
            abduction	-2.8
            abductions	-2
            abhor	-2
            abhorred	-2.4
            abhorrent	-3.1
            abhors	-2.9
            abilities	1
            ability	1.3
            aboard	0.1
            absentee	-1.1
            absentees	-0.8
            absolve	1.2
            absolved	1.5
            absolves	1.3
            absolving	1.6
            abuse	-3.2
            abused	-2.3
            abuser	-2.6
            abusers	-2.6
            abuses	-2.6
            abusing	-2
            abusive	-3.2
            abusively	-2.8
            abusiveness	-2.5
            abusivenesses	-3
            accept	1.6
            acceptabilities	1.6
            acceptability	1.1
            acceptable	1.3
            acceptableness	1.3
            acceptably	1.5
            acceptance	2
            acceptances	1.7
            acceptant	1.6
            acceptation	1.3
            acceptations	0.9
            accepted	1.1
            accepting	1.6
            accepts	1.3
            accident	-2.1
            accidental	-0.3
            accidentally	-1.4
            accidents	-1.3
            accomplish	1.8
            accomplished	1.9
            accomplishes	1.7
            accusation	-1
            accusations	-1.3
            accuse	-0.8
            accused	-1.2
            accuses	-1.4
            accusing	-0.7
            ache	-1.6
            ached	-1.6
            aches	-1
            achievable	1.3
            aching	-2.2
            acquit	0.8
            acquits	0.1
            acquitted	1
            acquitting	1.3
            acrimonious	-1.7
            active	1.7
            actively	1.3
            activeness	0.6
            activenesses	0.8
            actives	1.1
            adequate	0.9
            admirability	2.4
            admirable	2.6
            admirableness	2.2
            admirably	2.5
            admiral	1.3
            admirals	1.5
            admiralties	1.6
            admiralty	1.2
            admiration	2.5
            admirations	1.6
            admire	2.1
            admired	2.3
            admirer	1.8
            admirers	1.7
            admires	1.5
            admiring	1.6
            admiringly	2.3
            admit	0.8
            admits	1.2
            admitted	0.4
            admonished	-1.9
            adopt	0.7
            adopts	0.7
            adorability	2.2
            adorable	2.2
            adorableness	2.5
            adorably	2.1
            adoration	2.9
            adorations	2.2
            adore	2.6
            adored	1.8
            adorer	1.7
            adorers	2.1
            adores	1.6
            adoring	2.6
            adoringly	2.4
            adorn	0.9
            adorned	0.8
            adorner	1.3
            adorners	0.9
            adorning	1
            adornment	1.3
            adornments	0.8
            adorns	0.5
            advanced	1
            advantage	1
            advantaged	1.4
            advantageous	1.5
            advantageously	1.9
            advantageousness	1.6
            advantages	1.5
            advantaging	1.6
            adventure	1.3
            adventured	1.3
            adventurer	1.2
            adventurers	0.9
            adventures	1.4
            adventuresome	1.7
            adventuresomeness	1.3
            adventuress	0.8
            adventuresses	1.4
            adventuring	2.3
            adventurism	1.5
            adventurist	1.4
            adventuristic	1.7
            adventurists	1.2
            adventurous	1.4
            adventurously	1.3
            adventurousness	1.8
            adversarial	-1.5
            adversaries	-1
            adversary	-0.8
            adversative	-1.2
            adversatively	-0.1
            adversatives	-1
            adverse	-1.5
            adversely	-0.8
            adverseness	-0.6
            adversities	-1.5
            adversity	-1.8
            affected	-0.6
            affection	2.4
            affectional	1.9
            affectionally	1.5
            affectionate	1.9
            affectionately	2.2
            affectioned	1.8
            affectionless	-2
            affections	1.5
            afflicted	-1.5
            affronted	0.2
            aggravate	-2.5
            aggravated	-1.9
            aggravates	-1.9
            aggravating	-1.2
            aggress	-1.3
            aggressed	-1.4
            aggresses	-0.5
            aggressing	-0.6
            aggression	-1.2
            aggressions	-1.3
            aggressive	-0.6
            aggressively	-1.3
            aggressiveness	-1.8
            aggressivities	-1.4
            aggressivity	-0.6
            aggressor	-0.8
            aggressors	-0.9
            aghast	-1.9
            agitate	-1.7
            agitated	-2
            agitatedly	-1.6
            agitates	-1.4
            agitating	-1.8
            agitation	-1
            agitational	-1.2
            agitations	-1.3
            agitative	-1.3
            agitato	-0.1
            agitator	-1.4
            agitators	-2.1
            agog	1.9
            agonise	-2.1
            agonised	-2.3
            agonises	-2.4
            agonising	-1.5
            agonize	-2.3
            agonized	-2.2
            agonizes	-2.3
            agonizing	-2.7
            agonizingly	-2.3
            agony	-1.8
            agree	1.5
            agreeability	1.9
            agreeable	1.8
            agreeableness	1.8
            agreeablenesses	1.3
            agreeably	1.6
            agreed	1.1
            agreeing	1.4
            agreement	2.2
            agreements	1.1
            agrees	0.8
            alarm	-1.4
            alarmed	-1.4
            alarming	-0.5
            alarmingly	-2.6
            alarmism	-0.3
            alarmists	-1.1
            alarms	-1.1
            alas	-1.1
            alert	1.2
            alienation	-1.1
            alive	1.6
            allergic	-1.2
            allow	0.9
            alone	-1
            alright	1
            amaze	2.5
            amazed	2.2
            amazedly	2.1
            amazement	2.5
            amazements	2.2
            amazes	2.2
            amazing	2.8
            amazon	0.7
            amazonite	0.2
            amazons	-0.1
            amazonstone	1
            amazonstones	0.2
            ambitious	2.1
            ambivalent	0.5
            amor	3
            amoral	-1.6
            amoralism	-0.7
            amoralisms	-0.7
            amoralities	-1.2
            amorality	-1.5
            amorally	-1
            amoretti	0.2
            amoretto	0.6
            amorettos	0.3
            amorino	1.2
            amorist	1.6
            amoristic	1
            amorists	0.1
            amoroso	2.3
            amorous	1.8
            amorously	2.3
            amorousness	2
            amorphous	-0.2
            amorphously	0.1
            amorphousness	0.3
            amort	-2.1
            amortise	0.5
            amortised	-0.2
            amortises	0.1
            amortizable	0.5
            amortization	0.6
            amortizations	0.2
            amortize	-0.1
            amortized	0.8
            amortizes	0.6
            amortizing	0.8
            amusable	0.7
            amuse	1.7
            amused	1.8
            amusedly	2.2
            amusement	1.5
            amusements	1.5
            amuser	1.1
            amusers	1.3
            amuses	1.7
            amusia	0.3
            amusias	-0.4
            amusing	1.6
            amusingly	0.8
            amusingness	1.8
            amusive	1.7
            anger	-2.7
            angered	-2.3
            angering	-2.2
            angerly	-1.9
            angers	-2.3
            angrier	-2.3
            angriest	-3.1
            angrily	-1.8
            angriness	-1.7
            angry	-2.3
            anguish	-2.9
            anguished	-1.8
            anguishes	-2.1
            anguishing	-2.7
            animosity	-1.9
            annoy	-1.9
            annoyance	-1.3
            annoyances	-1.8
            annoyed	-1.6
            annoyer	-2.2
            annoyers	-1.5
            annoying	-1.7
            annoys	-1.8
            antagonism	-1.9
            antagonisms	-1.2
            antagonist	-1.9
            antagonistic	-1.7
            antagonistically	-2.2
            antagonists	-1.7
            antagonize	-2
            antagonized	-1.4
            antagonizes	-0.5
            antagonizing	-2.7
            anti	-1.3
            anticipation	0.4
            anxieties	-0.6
            anxiety	-0.7
            anxious	-1
            anxiously	-0.9
            anxiousness	-1
            aok	2
            apathetic	-1.2
            apathetically	-0.4
            apathies	-0.6
            apathy	-1.2
            apeshit	-0.9
            apocalyptic	-3.4
            apologise	1.6
            apologised	0.4
            apologises	0.8
            apologising	0.2
            apologize	0.4
            apologized	1.3
            apologizes	1.5
            apologizing	-0.3
            apology	0.2
            appall	-2.4
            appalled	-2
            appalling	-1.5
            appallingly	-2
            appalls	-1.9
            appease	1.1
            appeased	0.9
            appeases	0.9
            appeasing	1
            applaud	2
            applauded	1.5
            applauding	2.1
            applauds	1.4
            applause	1.8
            appreciate	1.7
            appreciated	2.3
            appreciates	2.3
            appreciating	1.9
            appreciation	2.3
            appreciations	1.7
            appreciative	2.6
            appreciatively	1.8
            appreciativeness	1.6
            appreciator	2.6
            appreciators	1.5
            appreciatory	1.7
            apprehensible	1.1
            apprehensibly	-0.2
            apprehension	-2.1
            apprehensions	-0.9
            apprehensively	-0.3
            apprehensiveness	-0.7
            approval	2.1
            approved	1.8
            approves	1.7
            ardent	2.1
            arguable	-1
            arguably	-1
            argue	-1.4
            argued	-1.5
            arguer	-1.6
            arguers	-1.4
            argues	-1.6
            arguing	-2
            argument	-1.5
            argumentative	-1.5
            argumentatively	-1.8
            argumentive	-1.5
            arguments	-1.7
            arrest	-1.4
            arrested	-2.1
            arrests	-1.9
            arrogance	-2.4
            arrogances	-1.9
            arrogant	-2.2
            arrogantly	-1.8
            ashamed	-2.1
            ashamedly	-1.7
            ass	-2.5
            assassination	-2.9
            assassinations	-2.7
            assault	-2.8
            assaulted	-2.4
            assaulting	-2.3
            assaultive	-2.8
            assaults	-2.5
            asset	1.5
            assets	0.7
            assfucking	-2.5
            assholes	-2.8
            assurance	1.4
            assurances	1.4
            assure	1.4
            assured	1.5
            assuredly	1.6
            assuredness	1.4
            assurer	0.9
            assurers	1.1
            assures	1.3
            assurgent	1.3
            assuring	1.6
            assuror	0.5
            assurors	0.7
            astonished	1.6
            astound	1.7
            astounded	1.8
            astounding	1.8
            astoundingly	2.1
            astounds	2.1
            attachment	1.2
            attachments	1.1
            attack	-2.1
            attacked	-2
            attacker	-2.7
            attackers	-2.7
            attacking	-2
            attacks	-1.9
            attract	1.5
            attractancy	0.9
            attractant	1.3
            attractants	1.4
            attracted	1.8
            attracting	2.1
            attraction	2
            attractions	1.8
            attractive	1.9
            attractively	2.2
            attractiveness	1.8
            attractivenesses	2.1
            attractor	1.2
            attractors	1.2
            attracts	1.7
            audacious	0.9
            authority	0.3
            aversion	-1.9
            aversions	-1.1
            aversive	-1.6
            aversively	-0.8
            avert	-0.7
            averted	-0.3
            averts	-0.4
            avid	1.2
            avoid	-1.2
            avoidance	-1.7
            avoidances	-1.1
            avoided	-1.4
            avoider	-1.8
            avoiders	-1.4
            avoiding	-1.4
            avoids	-0.7
            await	0.4
            awaited	-0.1
            awaits	0.3
            award	2.5
            awardable	2.4
            awarded	1.7
            awardee	1.8
            awardees	1.2
            awarder	0.9
            awarders	1.3
            awarding	1.9
            awards	2
            awesome	3.1
            awful	-2
            awkward	-0.6
            awkwardly	-1.3
            awkwardness	-0.7
            axe	-0.4
            axed	-1.3
            backed	0.1
            backing	0.1
            backs	-0.2
            bad	-2.5
            badass	-0.6
            badly	-2.1
            bailout	-0.4
            bamboozle	-1.5
            bamboozled	-1.5
            bamboozles	-1.5
            ban	-2.6
            banish	-1.9
            bankrupt	-2.6
            bankster	-2.1
            banned	-2
            bargain	0.8
            barrier	-0.5
            bashful	-0.1
            bashfully	0.2
            bashfulness	-0.8
            bastard	-2.5
            bastardies	-1.8
            bastardise	-2.1
            bastardised	-2.3
            bastardises	-2.3
            bastardising	-2.6
            bastardization	-2.4
            bastardizations	-2.1
            bastardize	-2.4
            bastardized	-2
            bastardizes	-1.8
            bastardizing	-2.3
            bastardly	-2.7
            bastards	-3
            bastardy	-2.7
            battle	-1.6
            battled	-1.2
            battlefield	-1.6
            battlefields	-0.9
            battlefront	-1.2
            battlefronts	-0.8
            battleground	-1.7
            battlegrounds	-0.6
            battlement	-0.4
            battlements	-0.4
            battler	-0.8
            battlers	-0.2
            battles	-1.6
            battleship	-0.1
            battleships	-0.5
            battlewagon	-0.3
            battlewagons	-0.5
            battling	-1.1
            beaten	-1.8
            beatific	1.8
            beating	-2
            beaut	1.6
            beauteous	2.5
            beauteously	2.6
            beauteousness	2.7
            beautician	1.2
            beauticians	0.4
            beauties	2.4
            beautification	1.9
            beautifications	2.4
            beautified	2.1
            beautifier	1.7
            beautifiers	1.7
            beautifies	1.8
            beautiful	2.9
            beautifuler	2.1
            beautifulest	2.6
            beautifully	2.7
            beautifulness	2.6
            beautify	2.3
            beautifying	2.3
            beauts	1.7
            beauty	2.8
            belittle	-1.9
            belittled	-2
            beloved	2.3
            benefic	1.4
            benefice	0.4
            beneficed	1.1
            beneficence	2.8
            beneficences	1.5
            beneficent	2.3
            beneficently	2.2
            benefices	1.1
            beneficial	1.9
            beneficially	2.4
            beneficialness	1.7
            beneficiaries	1.8
            beneficiary	2.1
            beneficiate	1
            beneficiation	0.4
            benefit	2
            benefits	1.6
            benefitted	1.7
            benefitting	1.9
            benevolence	1.7
            benevolences	1.9
            benevolent	2.7
            benevolently	1.4
            benevolentness	1.2
            benign	1.3
            benignancy	0.6
            benignant	2.2
            benignantly	1.1
            benignities	0.9
            benignity	1.3
            benignly	0.2
            bereave	-2.1
            bereaved	-2.1
            bereaves	-1.9
            bereaving	-1.3
            best	3.2
            betray	-3.2
            betrayal	-2.8
            betrayed	-3
            betraying	-2.5
            betrays	-2.5
            better	1.9
            bias	-0.4
            biased	-1.1
            bitch	-2.8
            bitched	-2.6
            bitcheries	-2.3
            bitchery	-2.7
            bitches	-2.9
            bitchier	-2
            bitchiest	-3
            bitchily	-2.6
            bitchiness	-2.6
            bitching	-1.1
            bitchy	-2.3
            bitter	-1.8
            bitterbrush	-0.2
            bitterbrushes	-0.6
            bittered	-1.8
            bitterer	-1.9
            bitterest	-2.3
            bittering	-1.2
            bitterish	-1.6
            bitterly	-2
            bittern	-0.2
            bitterness	-1.7
            bitterns	-0.4
            bitterroots	-0.2
            bitters	-0.4
            bittersweet	-0.3
            bittersweetness	-0.6
            bittersweets	-0.2
            bitterweeds	-0.5
            bizarre	-1.3
            blah	-0.4
            blam	-0.2
            blamable	-1.8
            blamably	-1.8
            blame	-1.4
            blamed	-2.1
            blameful	-1.7
            blamefully	-1.6
            blameless	0.7
            blamelessly	0.9
            blamelessness	0.6
            blamer	-2.1
            blamers	-2
            blames	-1.7
            blameworthiness	-1.6
            blameworthy	-2.3
            blaming	-2.2
            bless	1.8
            blessed	2.9
            blesseder	2
            blessedest	2.8
            blessedly	1.7
            blessedness	1.6
            blesser	2.6
            blessers	1.9
            blesses	2.6
            blessing	2.2
            blessings	2.5
            blind	-1.7
            bliss	2.7
            blissful	2.9
            blithe	1.2
            block	-1.9
            blockbuster	2.9
            blocked	-1.1
            blocking	-1.6
            blocks	-0.9
            bloody	-1.9
            blurry	-0.4
            bold	1.6
            bolder	1.2
            boldest	1.6
            boldface	0.3
            boldfaced	-0.1
            boldfaces	0.1
            boldfacing	0.1
            boldly	1.5
            boldness	1.5
            boldnesses	0.9
            bolds	1.3
            bomb	-2.2
            bonus	2.5
            bonuses	2.6
            boost	1.7
            boosted	1.5
            boosting	1.4
            boosts	1.3
            bore	-1
            boreal	-0.3
            borecole	-0.2
            borecoles	-0.3
            bored	-1.1
            boredom	-1.3
            boredoms	-1.1
            boreen	0.1
            boreens	0.2
            boreholes	-0.2
            borer	-0.4
            borers	-1.2
            bores	-1.3
            borescopes	-0.1
            boresome	-1.3
            boring	-1.3
            bother	-1.4
            botheration	-1.7
            botherations	-1.3
            bothered	-1.3
            bothering	-1.6
            bothers	-0.8
            bothersome	-1.3
            boycott	-1.3
            boycotted	-1.7
            boycotting	-1.7
            boycotts	-1.4
            brainwashing	-1.5
            brave	2.4
            braved	1.9
            bravely	2.3
            braver	2.4
            braveries	2
            bravery	2.2
            braves	1.9
            bravest	2.3
            breathtaking	2
            bribe	-0.8
            bright	1.9
            brighten	1.9
            brightened	2.1
            brightener	1
            brighteners	1
            brightening	2.5
            brightens	1.5
            brighter	1.6
            brightest	3
            brightly	1.5
            brightness	1.6
            brightnesses	1.4
            brights	0.4
            brightwork	1.1
            brilliance	2.9
            brilliances	2.9
            brilliancies	2.3
            brilliancy	2.6
            brilliant	2.8
            brilliantine	0.8
            brilliantines	2
            brilliantly	3
            brilliants	1.9
            brisk	0.6
            broke	-1.8
            broken	-2.1
            brooding	0.1
            brutal	-3.1
            brutalise	-2.7
            brutalised	-2.9
            brutalises	-3.2
            brutalising	-2.8
            brutalities	-2.6
            brutality	-3
            brutalization	-2.1
            brutalizations	-2.3
            brutalize	-2.9
            brutalized	-2.4
            brutalizes	-3.2
            brutalizing	-3.4
            brutally	-3
            bullied	-3.1
            bullshit	-2.8
            bully	-2.2
            bullying	-2.9
            bummer	-1.6
            buoyant	0.9
            burden	-1.9
            burdened	-1.7
            burdener	-1.3
            burdeners	-1.7
            burdening	-1.4
            burdens	-1.5
            burdensome	-1.8
            bwahaha	0.4
            bwahahah	2.5
            calm	1.3
            calmative	1.1
            calmatives	0.5
            calmed	1.6
            calmer	1.5
            calmest	1.6
            calming	1.7
            calmly	1.3
            calmness	1.7
            calmnesses	1.6
            calmodulin	0.2
            calms	1.3
            can't stand	-2
            cancel	-1
            cancelled	-1
            cancelling	-0.8
            cancels	-0.9
            cancer	-3.4
            capable	1.6
            captivated	1.6
            care	2.2
            cared	1.8
            carefree	1.7
            careful	0.6
            carefully	0.5
            carefulness	2
            careless	-1.5
            carelessly	-1
            carelessness	-1.4
            carelessnesses	-1.6
            cares	2
            caring	2.2
            casual	0.8
            casually	0.7
            casualty	-2.4
            catastrophe	-3.4
            catastrophic	-2.2
            cautious	-0.4
            celebrate	2.7
            celebrated	2.7
            celebrates	2.7
            celebrating	2.7
            censor	-2
            censored	-0.6
            censors	-1.2
            certain	1.1
            certainly	1.4
            certainties	0.9
            certainty	1
            chagrin	-1.9
            chagrined	-1.4
            challenge	0.3
            challenged	-0.4
            challenger	0.5
            challengers	0.4
            challenges	0.3
            challenging	0.6
            challengingly	-0.6
            champ	2.1
            champac	-0.2
            champagne	1.2
            champagnes	0.5
            champaign	0.2
            champaigns	0.5
            champaks	-0.2
            champed	1
            champer	-0.1
            champers	0.5
            champerties	-0.1
            champertous	0.3
            champerty	-0.2
            champignon	0.4
            champignons	0.2
            champing	0.7
            champion	2.9
            championed	1.2
            championing	1.8
            champions	2.4
            championship	1.9
            championships	2.2
            champs	1.8
            champy	1
            chance	1
            chances	0.8
            chaos	-2.7
            chaotic	-2.2
            charged	-0.8
            charges	-1.1
            charitable	1.7
            charitableness	1.9
            charitablenesses	1.6
            charitably	1.4
            charities	2.2
            charity	1.8
            charm	1.7
            charmed	2
            charmer	1.9
            charmers	2.1
            charmeuse	0.3
            charmeuses	0.4
            charming	2.8
            charminger	1.5
            charmingest	2.4
            charmingly	2.2
            charmless	-1.8
            charms	1.9
            chastise	-2.5
            chastised	-2.2
            chastises	-1.7
            chastising	-1.7
            cheat	-2
            cheated	-2.3
            cheater	-2.5
            cheaters	-1.9
            cheating	-2.6
            cheats	-1.8
            cheer	2.3
            cheered	2.3
            cheerer	1.7
            cheerers	1.8
            cheerful	2.5
            cheerfuller	1.9
            cheerfullest	3.2
            cheerfully	2.1
            cheerfulness	2.1
            cheerier	2.6
            cheeriest	2.2
            cheerily	2.5
            cheeriness	2.5
            cheering	2.3
            cheerio	1.2
            cheerlead	1.7
            cheerleader	0.9
            cheerleaders	1.2
            cheerleading	1.2
            cheerleads	1.2
            cheerled	1.5
            cheerless	-1.7
            cheerlessly	-0.8
            cheerlessness	-1.7
            cheerly	2.4
            cheers	2.1
            cheery	2.6
            cherish	1.6
            cherishable	2
            cherished	2.3
            cherisher	2.2
            cherishers	1.9
            cherishes	2.2
            cherishing	2
            chic	1.1
            childish	-1.2
            chilling	-0.1
            choke	-2.5
            choked	-2.1
            chokes	-2
            choking	-2
            chuckle	1.7
            chuckled	1.2
            chucklehead	-1.9
            chuckleheaded	-1.3
            chuckleheads	-1.1
            chuckler	0.8
            chucklers	1.2
            chuckles	1.1
            chucklesome	1.1
            chuckling	1.4
            chucklingly	1.2
            clarifies	0.9
            clarity	1.7
            classy	1.9
            clean	1.7
            cleaner	0.7
            clear	1.6
            cleared	0.4
            clearly	1.7
            clears	0.3
            clever	2
            cleverer	2
            cleverest	2.6
            cleverish	1
            cleverly	2.3
            cleverness	2.3
            clevernesses	1.4
            clouded	-0.2
            clueless	-1.5
            cock	-0.6
            cocksucker	-3.1
            cocksuckers	-2.6
            cocky	-0.5
            coerced	-1.5
            collapse	-2.2
            collapsed	-1.1
            collapses	-1.2
            collapsing	-1.2
            collide	-0.3
            collides	-1.1
            colliding	-0.5
            collision	-1.5
            collisions	-1.1
            colluding	-1.2
            combat	-1.4
            combats	-0.8
            comedian	1.6
            comedians	1.2
            comedic	1.7
            comedically	2.1
            comedienne	0.6
            comediennes	1.6
            comedies	1.7
            comedo	0.3
            comedones	-0.8
            comedown	-0.8
            comedowns	-0.9
            comedy	1.5
            comfort	1.5
            comfortable	2.3
            comfortableness	1.3
            comfortably	1.8
            comforted	1.8
            comforter	1.9
            comforters	1.2
            comforting	1.7
            comfortingly	1.7
            comfortless	-1.8
            comforts	2.1
            commend	1.9
            commended	1.9
            commit	1.2
            commitment	1.6
            commitments	0.5
            commits	0.1
            committed	1.1
            committing	0.3
            compassion	2
            compassionate	2.2
            compassionated	1.6
            compassionately	1.7
            compassionateness	0.9
            compassionates	1.6
            compassionating	1.6
            compassionless	-2.6
            compelled	0.2
            compelling	0.9
            competent	1.3
            competitive	0.7
            complacent	-0.3
            complain	-1.5
            complainant	-0.7
            complainants	-1.1
            complained	-1.7
            complainer	-1.8
            complainers	-1.3
            complaining	-0.8
            complainingly	-1.7
            complains	-1.6
            complaint	-1.2
            complaints	-1.7
            compliment	2.1
            complimentarily	1.7
            complimentary	1.9
            complimented	1.8
            complimenting	2.3
            compliments	1.7
            comprehensive	1
            conciliate	1
            conciliated	1.1
            conciliates	1.1
            conciliating	1.3
            condemn	-1.6
            condemnation	-2.8
            condemned	-1.9
            condemns	-2.3
            confidence	2.3
            confident	2.2
            confidently	2.1
            conflict	-1.3
            conflicting	-1.7
            conflictive	-1.8
            conflicts	-1.6
            confront	-0.7
            confrontation	-1.3
            confrontational	-1.6
            confrontationist	-1
            confrontationists	-1.2
            confrontations	-1.5
            confronted	-0.8
            confronter	-0.3
            confronters	-1.3
            confronting	-0.6
            confronts	-0.9
            confuse	-0.9
            confused	-1.3
            confusedly	-0.6
            confusedness	-1.5
            confuses	-1.3
            confusing	-0.9
            confusingly	-1.4
            confusion	-1.2
            confusional	-1.2
            confusions	-0.9
            congrats	2.4
            congratulate	2.2
            congratulation	2.9
            congratulations	2.9
            consent	0.9
            consents	1
            considerate	1.9
            consolable	1.1
            conspiracy	-2.4
            constrained	-0.4
            contagion	-2
            contagions	-1.5
            contagious	-1.4
            contempt	-2.8
            contemptibilities	-2
            contemptibility	-0.9
            contemptible	-1.6
            contemptibleness	-1.9
            contemptibly	-1.4
            contempts	-1
            contemptuous	-2.2
            contemptuously	-2.4
            contemptuousness	-1.1
            contend	0.2
            contender	0.5
            contented	1.4
            contentedly	1.9
            contentedness	1.4
            contentious	-1.2
            contentment	1.5
            contestable	0.6
            contradict	-1.3
            contradictable	-1
            contradicted	-1.3
            contradicting	-1.3
            contradiction	-1
            contradictions	-1.3
            contradictious	-1.9
            contradictor	-1
            contradictories	-0.5
            contradictorily	-0.9
            contradictoriness	-1.4
            contradictors	-1.6
            contradictory	-1.4
            contradicts	-1.4
            controversial	-0.8
            controversially	-1.1
            convince	1
            convinced	1.7
            convincer	0.6
            convincers	0.3
            convinces	0.7
            convincing	1.7
            convincingly	1.6
            convincingness	0.7
            convivial	1.2
            cool	1.3
            cornered	-1.1
            corpse	-2.7
            costly	-0.4
            courage	2.2
            courageous	2.4
            courageously	2.3
            courageousness	2.1
            courteous	2.3
            courtesy	1.5
            cover-up	-1.2
            coward	-2
            cowardly	-1.6
            coziness	1.5
            cramp	-0.8
            crap	-1.6
            crappy	-2.6
            crash	-1.7
            craze	-0.6
            crazed	-0.5
            crazes	0.2
            crazier	-0.1
            craziest	-0.2
            crazily	-1.5
            craziness	-1.6
            crazinesses	-1
            crazing	-0.5
            crazy	-1.4
            crazyweed	0.8
            create	1.1
            created	1
            creates	1.1
            creatin	0.1
            creatine	0.2
            creating	1.2
            creatinine	0.4
            creation	1.1
            creationism	0.7
            creationisms	1.1
            creationist	0.8
            creationists	0.5
            creations	1.6
            creative	1.9
            creatively	1.5
            creativeness	1.8
            creativities	1.7
            creativity	1.6
            credit	1.6
            creditabilities	1.4
            creditability	1.9
            creditable	1.8
            creditableness	1.2
            creditably	1.7
            credited	1.5
            crediting	0.6
            creditor	-0.1
            credits	1.5
            creditworthiness	1.9
            creditworthy	2.4
            crestfallen	-2.5
            cried	-1.6
            cries	-1.7
            crime	-2.5
            criminal	-2.4
            criminals	-2.7
            crisis	-3.1
            critic	-1.1
            critical	-1.3
            criticise	-1.9
            criticised	-1.8
            criticises	-1.3
            criticising	-1.7
            criticism	-1.9
            criticisms	-0.9
            criticizable	-1
            criticize	-1.6
            criticized	-1.5
            criticizer	-1.5
            criticizers	-1.6
            criticizes	-1.4
            criticizing	-1.5
            critics	-1.2
            crude	-2.7
            crudely	-1.2
            crudeness	-2
            crudenesses	-2
            cruder	-2
            crudes	-1.1
            crudest	-2.4
            cruel	-2.8
            crueler	-2.3
            cruelest	-2.6
            crueller	-2.4
            cruellest	-2.9
            cruelly	-2.8
            cruelness	-2.9
            cruelties	-2.3
            cruelty	-2.9
            crush	-0.6
            crushed	-1.8
            crushes	-1.9
            crushing	-1.5
            cry	-2.1
            crying	-2.1
            cunt	-2.2
            cunts	-2.9
            curious	1.3
            curse	-2.5
            cut	-1.1
            cute	2
            cutely	1.3
            cuteness	2.3
            cutenesses	1.9
            cuter	2.3
            cutes	1.8
            cutesie	1
            cutesier	1.5
            cutesiest	2.2
            cutest	2.8
            cutesy	2.1
            cutey	2.1
            cuteys	1.5
            cutie	1.5
            cutiepie	2
            cuties	2.2
            cuts	-1.2
            cutting	-0.5
            cynic	-1.4
            cynical	-1.6
            cynically	-1.3
            cynicism	-1.7
            cynicisms	-1.7
            cynics	-0.3
            d-:	1.6
            d:	1.2
            d=	1.5
            damage	-2.2
            damaged	-1.9
            damager	-1.9
            damagers	-2
            damages	-1.9
            damaging	-2.3
            damagingly	-2
            damn	-1.7
            damnable	-1.7
            damnableness	-1.8
            damnably	-1.7
            damnation	-2.6
            damnations	-1.4
            damnatory	-2.6
            damned	-1.6
            damnedest	-0.5
            damnified	-2.8
            damnifies	-1.8
            damnify	-2.2
            damnifying	-2.4
            damning	-1.4
            damningly	-2
            damnit	-2.4
            damns	-2.2
            danger	-2.4
            dangered	-2.4
            dangering	-2.5
            dangerous	-2.1
            dangerously	-2
            dangerousness	-2
            dangers	-2.2
            daredevil	0.5
            daring	1.5
            daringly	2.1
            daringness	1.4
            darings	0.4
            darkest	-2.2
            darkness	-1
            darling	2.8
            darlingly	1.6
            darlingness	2.3
            darlings	2.2
            dauntless	2.3
            daze	-0.7
            dazed	-0.7
            dazedly	-0.4
            dazedness	-0.5
            dazes	-0.3
            dead	-3.3
            deadlock	-1.4
            deafening	-1.2
            dear	1.6
            dearer	1.9
            dearest	2.6
            dearie	2.2
            dearies	1
            dearly	1.8
            dearness	2
            dears	1.9
            dearth	-2.3
            dearths	-0.9
            deary	1.9
            death	-2.9
            debonair	0.8
            debt	-1.5
            decay	-1.7
            decayed	-1.6
            decayer	-1.6
            decayers	-1.6
            decaying	-1.7
            decays	-1.7
            deceit	-2
            deceitful	-1.9
            deceive	-1.7
            deceived	-1.9
            deceives	-1.6
            deceiving	-1.4
            deception	-1.9
            decisive	0.9
            dedicated	2
            defeat	-2
            defeated	-2.1
            defeater	-1.4
            defeaters	-0.9
            defeating	-1.6
            defeatism	-1.3
            defeatist	-1.7
            defeatists	-2.1
            defeats	-1.3
            defeature	-1.9
            defeatures	-1.5
            defect	-1.4
            defected	-1.7
            defecting	-1.8
            defection	-1.4
            defections	-1.5
            defective	-1.9
            defectively	-2.1
            defectiveness	-1.8
            defectives	-1.8
            defector	-1.9
            defectors	-1.3
            defects	-1.7
            defence	0.4
            defenceman	0.4
            defencemen	0.6
            defences	-0.2
            defender	0.4
            defenders	0.3
            defense	0.5
            defenseless	-1.4
            defenselessly	-1.1
            defenselessness	-1.3
            defenseman	0.1
            defensemen	-0.4
            defenses	0.7
            defensibility	0.4
            defensible	0.8
            defensibly	0.1
            defensive	0.1
            defensively	-0.6
            defensiveness	-0.4
            defensives	-0.3
            defer	-1.2
            deferring	-0.7
            defiant	-0.9
            deficit	-1.7
            definite	1.1
            definitely	1.7
            degradable	-1
            degradation	-2.4
            degradations	-1.5
            degradative	-2
            degrade	-1.9
            degraded	-1.8
            degrader	-2
            degraders	-2
            degrades	-2.1
            degrading	-2.8
            degradingly	-2.7
            dehumanize	-1.8
            dehumanized	-1.9
            dehumanizes	-1.5
            dehumanizing	-2.4
            deject	-2.2
            dejected	-2.2
            dejecting	-2.3
            dejects	-2
            delay	-1.3
            delayed	-0.9
            delectable	2.9
            delectables	1.4
            delectably	2.8
            delicate	0.2
            delicately	1
            delicates	0.6
            delicatessen	0.4
            delicatessens	0.4
            delicious	2.7
            deliciously	1.9
            deliciousness	1.8
            delight	2.9
            delighted	2.3
            delightedly	2.4
            delightedness	2.1
            delighter	2
            delighters	2.6
            delightful	2.8
            delightfully	2.7
            delightfulness	2.1
            delighting	1.6
            delights	2
            delightsome	2.3
            demand	-0.5
            demanded	-0.9
            demanding	-0.9
            demonstration	0.4
            demoralized	-1.6
            denied	-1.9
            denier	-1.5
            deniers	-1.1
            denies	-1.8
            denounce	-1.4
            denounces	-1.9
            deny	-1.4
            denying	-1.4
            depress	-2.2
            depressant	-1.6
            depressants	-1.6
            depressed	-2.3
            depresses	-2.2
            depressible	-1.7
            depressing	-1.6
            depressingly	-2.3
            depression	-2.7
            depressions	-2.2
            depressive	-1.6
            depressively	-2.1
            depressives	-1.5
            depressor	-1.8
            depressors	-1.7
            depressurization	-0.3
            depressurizations	-0.4
            depressurize	-0.5
            depressurized	-0.3
            depressurizes	-0.3
            depressurizing	-0.7
            deprival	-2.1
            deprivals	-1.2
            deprivation	-1.8
            deprivations	-1.8
            deprive	-2.1
            deprived	-2.1
            depriver	-1.6
            deprivers	-1.4
            deprives	-1.7
            depriving	-2
            derail	-1.2
            derailed	-1.4
            derails	-1.3
            deride	-1.1
            derided	-0.8
            derides	-1
            deriding	-1.5
            derision	-1.2
            desirable	1.3
            desire	1.7
            desired	1.1
            desirous	1.3
            despair	-1.3
            despaired	-2.7
            despairer	-1.3
            despairers	-1.3
            despairing	-2.3
            despairingly	-2.2
            despairs	-2.7
            desperate	-1.3
            desperately	-1.6
            desperateness	-1.5
            desperation	-2
            desperations	-2.2
            despise	-1.4
            despised	-1.7
            despisement	-2.4
            despisements	-2.5
            despiser	-1.8
            despisers	-1.6
            despises	-2
            despising	-2.7
            despondent	-2.1
            destroy	-2.5
            destroyed	-2.2
            destroyer	-2
            destroyers	-2.3
            destroying	-2.6
            destroys	-2.6
            destruct	-2.4
            destructed	-1.9
            destructibility	-1.8
            destructible	-1.5
            destructing	-2.5
            destruction	-2.7
            destructionist	-2.6
            destructionists	-2.1
            destructions	-2.3
            destructive	-3
            destructively	-2.4
            destructiveness	-2.4
            destructivity	-2.2
            destructs	-2.4
            detached	-0.5
            detain	-1.8
            detained	-1.7
            detention	-1.5
            determinable	0.9
            determinableness	0.2
            determinably	0.9
            determinacy	1
            determinant	0.2
            determinantal	-0.3
            determinate	0.8
            determinately	1.2
            determinateness	1.1
            determination	1.7
            determinations	0.8
            determinative	1.1
            determinatives	0.9
            determinator	1.1
            determined	1.4
            devastate	-3.1
            devastated	-3
            devastates	-2.8
            devastating	-3.3
            devastatingly	-2.4
            devastation	-1.8
            devastations	-1.9
            devastative	-3.2
            devastator	-2.8
            devastators	-2.9
            devil	-3.4
            deviled	-1.6
            devilfish	-0.8
            devilfishes	-0.6
            deviling	-2.2
            devilish	-2.1
            devilishly	-1.6
            devilishness	-2.3
            devilkin	-2.4
            devilled	-2.3
            devilling	-1.8
            devilment	-1.9
            devilments	-1.1
            devilries	-1.6
            devilry	-2.8
            devils	-2.7
            deviltries	-1.5
            deviltry	-2.8
            devilwood	-0.8
            devilwoods	-1
            devote	1.4
            devoted	1.7
            devotedly	1.6
            devotedness	2
            devotee	1.6
            devotees	0.5
            devotement	1.5
            devotements	1.1
            devotes	1.6
            devoting	2.1
            devotion	2
            devotional	1.2
            devotionally	2.2
            devotionals	1.2
            devotions	1.8
            diamond	1.4
            dick	-2.3
            dickhead	-3.1
            die	-2.9
            died	-2.6
            difficult	-1.5
            difficulties	-1.2
            difficultly	-1.7
            difficulty	-1.4
            diffident	-1
            dignified	2.2
            dignifies	2
            dignify	1.8
            dignifying	2.1
            dignitaries	0.6
            dignitary	1.9
            dignities	1.4
            dignity	1.7
            dilemma	-0.7
            dipshit	-2.1
            dire	-2
            direful	-3.1
            dirt	-1.4
            dirtier	-1.4
            dirtiest	-2.4
            dirty	-1.9
            disabling	-2.1
            disadvantage	-1.8
            disadvantaged	-1.7
            disadvantageous	-1.8
            disadvantageously	-2.1
            disadvantageousness	-1.6
            disadvantages	-1.7
            disagree	-1.6
            disagreeable	-1.7
            disagreeableness	-1.7
            disagreeablenesses	-1.9
            disagreeably	-1.5
            disagreed	-1.3
            disagreeing	-1.4
            disagreement	-1.5
            disagreements	-1.8
            disagrees	-1.3
            disappear	-0.9
            disappeared	-0.9
            disappears	-1.4
            disappoint	-1.7
            disappointed	-2.1
            disappointedly	-1.7
            disappointing	-2.2
            disappointingly	-1.9
            disappointment	-2.3
            disappointments	-2
            disappoints	-1.6
            disaster	-3.1
            disasters	-2.6
            disastrous	-2.9
            disbelieve	-1.2
            discard	-1
            discarded	-1.4
            discarding	-0.7
            discards	-1
            discomfort	-1.8
            discomfortable	-1.6
            discomforted	-1.6
            discomforting	-1.6
            discomforts	-1.3
            disconsolate	-2.3
            disconsolation	-1.7
            discontented	-1.8
            discord	-1.7
            discounted	0.2
            discourage	-1.8
            discourageable	-1.2
            discouraged	-1.7
            discouragement	-2
            discouragements	-1.8
            discourager	-1.7
            discouragers	-1.9
            discourages	-1.9
            discouraging	-1.9
            discouragingly	-1.8
            discredited	-1.9
            disdain	-2.1
            disgrace	-2.2
            disgraced	-2
            disguise	-1
            disguised	-1.1
            disguises	-1
            disguising	-1.3
            disgust	-2.9
            disgusted	-2.4
            disgustedly	-3
            disgustful	-2.6
            disgusting	-2.4
            disgustingly	-2.9
            disgusts	-2.1
            dishearten	-2
            disheartened	-2.2
            disheartening	-1.8
            dishearteningly	-2
            disheartenment	-2.3
            disheartenments	-2.2
            disheartens	-2.2
            dishonest	-2.7
            disillusion	-1
            disillusioned	-1.9
            disillusioning	-1.3
            disillusionment	-1.7
            disillusionments	-1.5
            disillusions	-1.6
            disinclined	-1.1
            disjointed	-1.3
            dislike	-1.6
            disliked	-1.7
            dislikes	-1.7
            disliking	-1.3
            dismal	-3
            dismay	-1.8
            dismayed	-1.9
            dismaying	-2.2
            dismayingly	-1.9
            dismays	-1.8
            disorder	-1.7
            disorganized	-1.2
            disoriented	-1.5
            disparage	-2
            disparaged	-1.4
            disparages	-1.6
            disparaging	-2.2
            displeased	-1.9
            dispute	-1.7
            disputed	-1.4
            disputes	-1.1
            disputing	-1.7
            disqualified	-1.8
            disquiet	-1.3
            disregard	-1.1
            disregarded	-1.6
            disregarding	-0.9
            disregards	-1.4
            disrespect	-1.8
            disrespected	-2
            disruption	-1.5
            disruptions	-1.4
            disruptive	-1.3
            dissatisfaction	-2.2
            dissatisfactions	-1.9
            dissatisfactory	-2
            dissatisfied	-1.6
            dissatisfies	-1.8
            dissatisfy	-2.2
            dissatisfying	-2.4
            distort	-1.3
            distorted	-1.7
            distorting	-1.1
            distorts	-1.4
            distract	-1.2
            distractable	-1.3
            distracted	-1.4
            distractedly	-0.9
            distractibility	-1.3
            distractible	-1.5
            distracting	-1.2
            distractingly	-1.4
            distraction	-1.6
            distractions	-1
            distractive	-1.6
            distracts	-1.3
            distraught	-2.6
            distress	-2.4
            distressed	-1.8
            distresses	-1.6
            distressful	-2.2
            distressfully	-1.7
            distressfulness	-2.4
            distressing	-1.7
            distressingly	-2.2
            distrust	-1.8
            distrusted	-2.4
            distrustful	-2.1
            distrustfully	-1.8
            distrustfulness	-1.6
            distrusting	-2.1
            distrusts	-1.3
            disturb	-1.7
            disturbance	-1.6
            disturbances	-1.4
            disturbed	-1.6
            disturber	-1.4
            disturbers	-2.1
            disturbing	-2.3
            disturbingly	-2.3
            disturbs	-1.9
            dithering	-0.5
            divination	1.7
            divinations	1.1
            divinatory	1.6
            divine	2.6
            divined	0.8
            divinely	2.9
            diviner	0.3
            diviners	1.2
            divines	0.8
            divinest	2.7
            diving	0.3
            divining	0.9
            divinise	0.5
            divinities	1.8
            divinity	2.7
            divinize	2.3
            dizzy	-0.9
            dodging	-0.4
            dodgy	-0.9
            dolorous	-2.2
            dominance	0.8
            dominances	-0.1
            dominantly	0.2
            dominants	0.2
            dominate	-0.5
            dominates	0.2
            dominating	-1.2
            domination	-0.2
            dominations	-0.3
            dominative	-0.7
            dominators	-0.4
            dominatrices	-0.2
            dominatrix	-0.5
            dominatrixes	0.6
            doom	-1.7
            doomed	-3.2
            doomful	-2.1
            dooming	-2.8
            dooms	-1.1
            doomsayer	-0.7
            doomsayers	-1.7
            doomsaying	-1.5
            doomsayings	-1.5
            doomsday	-2.8
            doomsdayer	-2.2
            doomsdays	-2.4
            doomster	-2.2
            doomsters	-1.6
            doomy	-1.1
            dork	-1.4
            dorkier	-1.1
            dorkiest	-1.2
            dorks	-0.5
            dorky	-1.1
            doubt	-1.5
            doubtable	-1.5
            doubted	-1.1
            doubter	-1.6
            doubters	-1.3
            doubtful	-1.4
            doubtfully	-1.2
            doubtfulness	-1.2
            doubting	-1.4
            doubtingly	-1.4
            doubtless	0.9
            doubtlessly	1.2
            doubtlessness	0.8
            doubts	-1.2
            douche	-1.5
            douchebag	-3
            downcast	-1.8
            downhearted	-2.3
            downside	-1
            drag	-0.9
            dragged	-0.2
            drags	-0.7
            drained	-1.5
            dread	-2
            dreaded	-2.7
            dreadful	-1.9
            dreadfully	-2.7
            dreadfulness	-3.2
            dreadfuls	-2.4
            dreading	-2.4
            dreadlock	-0.4
            dreadlocks	-0.2
            dreadnought	-0.6
            dreadnoughts	-0.4
            dreads	-1.4
            dream	1
            dreams	1.7
            dreary	-1.4
            droopy	-0.8
            drop	-1.1
            drown	-2.7
            drowned	-2.9
            drowns	-2.2
            drunk	-1.4
            dubious	-1.5
            dud	-1
            dull	-1.7
            dullard	-1.6
            dullards	-1.8
            dulled	-1.5
            duller	-1.7
            dullest	-1.7
            dulling	-1.1
            dullish	-1.1
            dullness	-1.4
            dullnesses	-1.9
            dulls	-1
            dullsville	-2.4
            dully	-1.1
            dumb	-2.3
            dumbass	-2.6
            dumbbell	-0.8
            dumbbells	-0.2
            dumbcane	-0.3
            dumbcanes	-0.6
            dumbed	-1.4
            dumber	-1.5
            dumbest	-2.3
            dumbfound	-0.1
            dumbfounded	-1.6
            dumbfounder	-1
            dumbfounders	-1
            dumbfounding	-0.8
            dumbfounds	-0.3
            dumbhead	-2.6
            dumbheads	-1.9
            dumbing	-0.5
            dumbly	-1.3
            dumbness	-1.9
            dumbs	-1.5
            dumbstruck	-1
            dumbwaiter	0.2
            dumbwaiters	-0.1
            dump	-1.6
            dumpcart	-0.6
            dumped	-1.7
            dumper	-1.2
            dumpers	-0.8
            dumpier	-1.4
            dumpiest	-1.6
            dumpiness	-1.2
            dumping	-1.3
            dumpings	-1.1
            dumpish	-1.8
            dumpling	0.4
            dumplings	-0.3
            dumps	-1.7
            dumpster	-0.6
            dumpsters	-1
            dumpy	-1.7
            dupe	-1.5
            duped	-1.8
            dwell	0.5
            dwelled	0.4
            dweller	0.3
            dwellers	-0.3
            dwelling	0.1
            dwells	-0.1
            dynamic	1.6
            dynamical	1.2
            dynamically	1.5
            dynamics	1.1
            dynamism	1.6
            dynamisms	1.2
            dynamist	1.4
            dynamistic	1.5
            dynamists	0.9
            dynamite	0.7
            dynamited	-0.9
            dynamiter	-1.2
            dynamiters	0.4
            dynamites	-0.3
            dynamitic	0.9
            dynamiting	0.2
            dynamometer	0.3
            dynamometers	0.3
            dynamometric	0.3
            dynamometry	0.6
            dynamos	0.3
            dynamotor	0.6
            dysfunction	-1.8
            eager	1.5
            eagerly	1.6
            eagerness	1.7
            eagers	1.6
            earnest	2.3
            ease	1.5
            eased	1.2
            easeful	1.5
            easefully	1.4
            easel	0.3
            easement	1.6
            easements	0.4
            eases	1.3
            easier	1.8
            easiest	1.8
            easily	1.4
            easiness	1.6
            easing	1
            easy	1.9
            easygoing	1.3
            easygoingness	1.5
            ecstacy	3.3
            ecstasies	2.3
            ecstasy	2.9
            ecstatic	2.3
            ecstatically	2.8
            ecstatics	2.9
            eerie	-1.5
            eery	-0.9
            effective	2.1
            effectively	1.9
            efficiencies	1.6
            efficiency	1.5
            efficient	1.8
            efficiently	1.7
            effin	-2.3
            egotism	-1.4
            egotisms	-1
            egotist	-2.3
            egotistic	-1.4
            egotistical	-0.9
            egotistically	-1.8
            egotists	-1.7
            elated	3.2
            elation	1.5
            elegance	2.1
            elegances	1.8
            elegancies	1.6
            elegancy	2.1
            elegant	2.1
            elegantly	1.9
            embarrass	-1.2
            embarrassable	-1.6
            embarrassed	-1.5
            embarrassedly	-1.1
            embarrasses	-1.7
            embarrassing	-1.6
            embarrassingly	-1.7
            embarrassment	-1.9
            embarrassments	-1.7
            embittered	-0.4
            embrace	1.3
            emergency	-1.6
            emotional	0.6
            empathetic	1.7
            emptied	-0.7
            emptier	-0.7
            emptiers	-0.7
            empties	-0.7
            emptiest	-1.8
            emptily	-1
            emptiness	-1.9
            emptinesses	-1.5
            emptins	-0.3
            empty	-0.8
            emptying	-0.6
            enchanted	1.6
            encourage	2.3
            encouraged	1.5
            encouragement	1.8
            encouragements	2.1
            encourager	1.5
            encouragers	1.5
            encourages	1.9
            encouraging	2.4
            encouragingly	2
            endorse	1.3
            endorsed	1
            endorsement	1.3
            endorses	1.4
            enemies	-2.2
            enemy	-2.5
            energetic	1.9
            energetically	1.8
            energetics	0.3
            energies	0.9
            energise	2.2
            energised	2.1
            energises	2.2
            energising	1.9
            energization	1.6
            energizations	1.5
            energize	2.1
            energized	2.3
            energizer	2.1
            energizers	1.7
            energizes	2.1
            energizing	2
            energy	1.1
            engage	1.4
            engaged	1.7
            engagement	2
            engagements	0.6
            engager	1.1
            engagers	1
            engages	1
            engaging	1.4
            engagingly	1.5
            engrossed	0.6
            enjoy	2.2
            enjoyable	1.9
            enjoyableness	1.9
            enjoyably	1.8
            enjoyed	2.3
            enjoyer	2.2
            enjoyers	2.2
            enjoying	2.4
            enjoyment	2.6
            enjoyments	2
            enjoys	2.3
            enlighten	2.3
            enlightened	2.2
            enlightening	2.3
            enlightens	1.7
            ennui	-1.2
            enrage	-2.6
            enraged	-1.7
            enrages	-1.8
            enraging	-2.8
            enrapture	3
            enslave	-3.1
            enslaved	-1.7
            enslaves	-1.6
            ensure	1.6
            ensuring	1.1
            enterprising	2.3
            entertain	1.3
            entertained	1.7
            entertainer	1.6
            entertainers	1
            entertaining	1.9
            entertainingly	1.9
            entertainment	1.8
            entertainments	2.3
            entertains	2.4
            enthral	0.4
            enthuse	1.6
            enthused	2
            enthuses	1.7
            enthusiasm	1.9
            enthusiasms	2
            enthusiast	1.5
            enthusiastic	2.2
            enthusiastically	2.6
            enthusiasts	1.4
            enthusing	1.9
            entitled	1.1
            entrusted	0.8
            envied	-1.1
            envier	-1
            enviers	-1.1
            envies	-0.8
            envious	-1.1
            envy	-1.1
            envying	-0.8
            envyingly	-1.3
            erroneous	-1.8
            error	-1.7
            errors	-1.4
            escape	0.7
            escapes	0.5
            escaping	0.2
            esteemed	1.9
            ethical	2.3
            euphoria	3.3
            euphoric	3.2
            eviction	-2
            evil	-3.4
            evildoer	-3.1
            evildoers	-2.4
            evildoing	-3.1
            evildoings	-2.5
            eviler	-2.1
            evilest	-2.5
            eviller	-2.9
            evillest	-3.3
            evilly	-3.4
            evilness	-3.1
            evils	-2.7
            exaggerate	-0.6
            exaggerated	-0.4
            exaggerates	-0.6
            exaggerating	-0.7
            exasperated	-1.8
            excel	2
            excelled	2.2
            excellence	3.1
            excellences	2.5
            excellencies	2.4
            excellency	2.5
            excellent	2.7
            excellently	3.1
            excelling	2.5
            excels	2.5
            excelsior	0.7
            excitabilities	1.5
            excitability	1.2
            excitable	1.5
            excitableness	1
            excitant	1.8
            excitants	1.2
            excitation	1.8
            excitations	1.8
            excitative	0.3
            excitatory	1.1
            excite	2.1
            excited	1.4
            excitedly	2.3
            excitement	2.2
            excitements	1.9
            exciter	1.9
            exciters	1.4
            excites	2.1
            exciting	2.2
            excitingly	1.9
            exciton	0.3
            excitonic	0.2
            excitons	0.8
            excitor	0.5
            exclude	-0.9
            excluded	-1.4
            exclusion	-1.2
            exclusive	0.5
            excruciate	-2.7
            excruciated	-1.3
            excruciates	-1
            excruciating	-3.3
            excruciatingly	-2.9
            excruciation	-3.4
            excruciations	-1.9
            excuse	0.3
            exempt	0.4
            exhaust	-1.2
            exhausted	-1.5
            exhauster	-1.3
            exhausters	-1.3
            exhaustibility	-0.8
            exhaustible	-1
            exhausting	-1.5
            exhaustion	-1.5
            exhaustions	-1.1
            exhaustive	-0.5
            exhaustively	-0.7
            exhaustiveness	-1.1
            exhaustless	0.2
            exhaustlessness	0.9
            exhausts	-1.1
            exhilarated	3
            exhilarates	2.8
            exhilarating	1.7
            exonerate	1.8
            exonerated	1.8
            exonerates	1.6
            exonerating	1
            expand	1.3
            expands	0.4
            expel	-1.9
            expelled	-1
            expelling	-1.6
            expels	-1.6
            exploit	-0.4
            exploited	-2
            exploiting	-1.9
            exploits	-1.4
            exploration	0.9
            explorations	0.3
            expose	-0.6
            exposed	-0.3
            exposes	-0.5
            exposing	-1.1
            extend	0.7
            extends	0.5
            exuberant	2.8
            exultant	3
            exultantly	1.4
            fab	2
            fabulous	2.4
            fabulousness	2.8
            fad	0.9
            fag	-2.1
            faggot	-3.4
            faggots	-3.2
            fail	-2.5
            failed	-2.3
            failing	-2.3
            failingly	-1.4
            failings	-2.2
            faille	0.1
            fails	-1.8
            failure	-2.3
            failures	-2
            fainthearted	-0.3
            fair	1.3
            faith	1.8
            faithed	1.3
            faithful	1.9
            faithfully	1.8
            faithfulness	1.9
            faithless	-1
            faithlessly	-0.9
            faithlessness	-1.8
            faiths	1.8
            fake	-2.1
            fakes	-1.8
            faking	-1.8
            fallen	-1.5
            falling	-0.6
            falsified	-1.6
            falsify	-2
            fame	1.9
            fan	1.3
            fantastic	2.6
            fantastical	2
            fantasticalities	2.1
            fantasticality	1.7
            fantasticalness	1.3
            fantasticate	1.5
            fantastico	0.4
            farce	-1.7
            fascinate	2.4
            fascinated	2.1
            fascinates	2
            fascination	2.2
            fascinating	2.5
            fascist	-2.6
            fascists	-0.8
            fatal	-2.5
            fatalism	-0.6
            fatalisms	-1.7
            fatalist	-0.5
            fatalistic	-1
            fatalists	-1.2
            fatalities	-2.9
            fatality	-3.5
            fatally	-3.2
            fatigue	-1
            fatigued	-1.4
            fatigues	-1.3
            fatiguing	-1.2
            fatiguingly	-1.5
            fault	-1.7
            faulted	-1.4
            faultfinder	-0.8
            faultfinders	-1.5
            faultfinding	-2.1
            faultier	-2.1
            faultiest	-2.1
            faultily	-2
            faultiness	-1.5
            faulting	-1.4
            faultless	2
            faultlessly	2
            faultlessness	1.1
            faults	-2.1
            faulty	-1.3
            fav	2
            fave	1.9
            favor	1.7
            favorable	2.1
            favorableness	2.2
            favorably	1.6
            favored	1.8
            favorer	1.3
            favorers	1.4
            favoring	1.8
            favorite	2
            favorited	1.7
            favorites	1.8
            favoritism	0.7
            favoritisms	0.7
            favors	1
            favour	1.9
            favoured	1.8
            favourer	1.6
            favourers	1.6
            favouring	1.3
            favours	1.8
            fear	-2.2
            feared	-2.2
            fearful	-2.2
            fearfuller	-2.2
            fearfullest	-2.5
            fearfully	-2.2
            fearfulness	-1.8
            fearing	-2.7
            fearless	1.9
            fearlessly	1.1
            fearlessness	1.1
            fears	-1.8
            fearsome	-1.7
            fed up	-1.8
            feeble	-1.2
            feeling	0.5
            felonies	-2.5
            felony	-2.5
            ferocious	-0.4
            ferociously	-1.1
            ferociousness	-1
            ferocities	-1
            ferocity	-0.7
            fervent	1.1
            fervid	0.5
            festival	2.2
            festivalgoer	1.3
            festivalgoers	1.2
            festivals	1.5
            festive	2
            festively	2.2
            festiveness	2.4
            festivities	2.1
            festivity	2.2
            feud	-1.4
            feudal	-0.8
            feudalism	-0.9
            feudalisms	-0.2
            feudalist	-0.9
            feudalistic	-1.1
            feudalities	-0.4
            feudality	-0.5
            feudalization	-0.3
            feudalize	-0.5
            feudalized	-0.8
            feudalizes	-0.1
            feudalizing	-0.7
            feudally	-0.6
            feudaries	-0.3
            feudary	-0.8
            feudatories	-0.5
            feudatory	-0.1
            feuded	-2.2
            feuding	-1.6
            feudist	-1.1
            feudists	-0.7
            feuds	-1.4
            fiasco	-2.3
            fidgety	-1.4
            fiery	-1.4
            fiesta	2.1
            fiestas	1.5
            fight	-1.6
            fighter	0.6
            fighters	-0.2
            fighting	-1.5
            fightings	-1.9
            fights	-1.7
            fine	0.8
            fire	-1.4
            fired	-2.6
            firing	-1.4
            fit	1.5
            fitness	1.1
            flagship	0.4
            flatter	0.4
            flattered	1.6
            flatterer	-0.3
            flatterers	0.3
            flatteries	1.2
            flattering	1.3
            flatteringly	1
            flatters	0.6
            flattery	0.4
            flawless	2.3
            flawlessly	0.8
            flees	-0.7
            flexibilities	1
            flexibility	1.4
            flexible	0.9
            flexibly	1.3
            flirtation	1.7
            flirtations	-0.1
            flirtatious	0.5
            flirtatiously	-0.1
            flirtatiousness	0.6
            flirted	-0.2
            flirter	-0.4
            flirters	0.6
            flirtier	-0.1
            flirtiest	0.4
            flirting	0.8
            flirts	0.7
            flirty	0.6
            flop	-1.4
            flops	-1.4
            flu	-1.6
            flunk	-1.3
            flunked	-2.1
            flunker	-1.9
            flunkers	-1.6
            flunkey	-1.8
            flunkeys	-0.6
            flunkies	-1.4
            flunking	-1.5
            flunks	-1.8
            flunky	-1.8
            flustered	-1
            focused	1.6
            foe	-1.9
            foehns	0.2
            foeman	-1.8
            foemen	-0.3
            foes	-2
            foetal	-0.1
            foetid	-2.3
            foetor	-3
            foetors	-2.1
            foetus	0.2
            foetuses	0.2
            fond	1.9
            fondly	1.9
            fondness	2.5
            fool	-1.9
            fooled	-1.6
            fooleries	-1.8
            foolery	-1.8
            foolfish	-0.8
            foolfishes	-0.4
            foolhardier	-1.5
            foolhardiest	-1.3
            foolhardily	-1
            foolhardiness	-1.6
            foolhardy	-1.4
            fooling	-1.7
            foolish	-1.1
            foolisher	-1.7
            foolishest	-1.4
            foolishly	-1.8
            foolishness	-1.8
            foolishnesses	-2
            foolproof	1.6
            fools	-2.2
            foolscaps	-0.8
            forbid	-1.3
            forbiddance	-1.4
            forbiddances	-1
            forbidden	-1.8
            forbidder	-1.6
            forbidders	-1.5
            forbidding	-1.9
            forbiddingly	-1.9
            forbids	-1.3
            forced	-2
            foreclosure	-0.5
            foreclosures	-2.4
            forgave	1.4
            forget	-0.9
            forgetful	-1.1
            forgivable	1.7
            forgivably	1.6
            forgive	1.1
            forgiven	1.6
            forgiveness	1.1
            forgiver	1.7
            forgivers	1.2
            forgives	1.7
            forgiving	1.9
            forgivingly	1.4
            forgivingness	1.8
            forgotten	-0.9
            fortunate	1.9
            fought	-1.3
            foughten	-1.9
            frantic	-1.9
            frantically	-1.4
            franticness	-0.7
            fraud	-2.8
            frauds	-2.3
            fraudster	-2.5
            fraudsters	-2.4
            fraudulence	-2.3
            fraudulent	-2.2
            freak	-1.9
            freaked	-1.2
            freakier	-1.3
            freakiest	-1.6
            freakiness	-1.4
            freaking	-1.8
            freakish	-2.1
            freakishly	-0.8
            freakishness	-1.4
            freakout	-1.8
            freakouts	-1.5
            freaks	-0.4
            freaky	-1.5
            free	2.3
            freebase	-0.1
            freebased	0.8
            freebases	0.8
            freebasing	-0.4
            freebee	1.3
            freebees	1.3
            freebie	1.8
            freebies	1.8
            freeboard	0.3
            freeboards	0.7
            freeboot	-0.7
            freebooter	-1.7
            freebooters	-0.2
            freebooting	-0.8
            freeborn	1.2
            freed	1.7
            freedman	1.1
            freedmen	0.7
            freedom	3.2
            freedoms	1.2
            freedwoman	1.6
            freedwomen	1.3
            freeform	0.9
            freehand	0.5
            freehanded	1.4
            freehearted	1.5
            freehold	0.7
            freeholder	0.5
            freeholders	0.1
            freeholds	1
            freeing	2.1
            freelance	1.2
            freelanced	0.7
            freelancer	1.1
            freelancers	0.4
            freelances	0.7
            freelancing	0.4
            freeload	-1.9
            freeloaded	-1.6
            freeloader	-0.7
            freeloaders	-0.1
            freeloading	-1.3
            freeloads	-1.3
            freely	1.9
            freeman	1.7
            freemartin	-0.5
            freemasonries	0.7
            freemasonry	0.3
            freemen	1.5
            freeness	1.6
            freenesses	1.7
            freer	1.1
            freers	1
            frees	1.2
            freesia	0.4
            freesias	0.4
            freest	1.6
            freestanding	1.1
            freestyle	0.7
            freestyler	0.4
            freestylers	0.8
            freestyles	0.3
            freethinker	1
            freethinkers	1
            freethinking	1.1
            freeware	0.7
            freeway	0.2
            freewheel	0.5
            freewheeled	0.3
            freewheeler	0.2
            freewheelers	-0.3
            freewheeling	0.5
            freewheelingly	0.8
            freewheels	0.6
            freewill	1
            freewriting	0.8
            freeze	0.2
            freezers	-0.1
            freezes	-0.1
            freezing	-0.4
            freezingly	-1.6
            frenzy	-1.3
            fresh	1.3
            friend	2.2
            friended	1.7
            friending	1.8
            friendless	-1.5
            friendlessness	-0.3
            friendlier	2
            friendlies	2.2
            friendliest	2.6
            friendlily	1.8
            friendliness	2
            friendly	2.2
            friends	2.1
            friendship	1.9
            friendships	1.6
            fright	-1.6
            frighted	-1.4
            frighten	-1.4
            frightened	-1.9
            frightening	-2.2
            frighteningly	-2.1
            frightens	-1.7
            frightful	-2.3
            frightfully	-1.7
            frightfulness	-1.9
            frighting	-1.5
            frights	-1.1
            frisky	1
            frowning	-1.4
            frustrate	-2
            frustrated	-2.4
            frustrates	-1.9
            frustrating	-1.9
            frustratingly	-2
            frustration	-2.1
            frustrations	-2
            fuck	-2.5
            fucked	-3.4
            fucker	-3.3
            fuckers	-2.9
            fuckface	-3.2
            fuckhead	-3.1
            fucks	-2.1
            fucktard	-3.1
            fud	-1.1
            fuked	-2.5
            fuking	-3.2
            fulfill	1.9
            fulfilled	1.8
            fulfills	1
            fume	-1.2
            fumed	-1.8
            fumeless	0.3
            fumelike	-0.7
            fumer	0.7
            fumers	-0.8
            fumes	-0.1
            fumet	0.4
            fumets	-0.4
            fumette	-0.6
            fuming	-2.7
            fun	2.3
            funeral	-1.5
            funerals	-1.6
            funky	-0.4
            funned	2.3
            funnel	0.1
            funneled	0.1
            funnelform	0.5
            funneling	-0.1
            funnelled	-0.1
            funnelling	0.1
            funnels	0.4
            funner	2.2
            funnest	2.9
            funnier	1.7
            funnies	1.3
            funniest	2.6
            funnily	1.9
            funniness	1.8
            funninesses	1.6
            funning	1.8
            funny	1.9
            funnyman	1.4
            funnymen	1.3
            furious	-2.7
            furiously	-1.9
            fury	-2.7
            futile	-1.9
            gag	-1.4
            gagged	-1.3
            gain	2.4
            gained	1.6
            gaining	1.8
            gains	1.4
            gallant	1.7
            gallantly	1.9
            gallantry	2.6
            geek	-0.8
            geekier	0.2
            geekiest	-0.1
            geeks	-0.4
            geeky	-0.6
            generosities	2.6
            generosity	2.3
            generous	2.3
            generously	1.8
            generousness	2.4
            genial	1.8
            gentle	1.9
            gentler	1.4
            gentlest	1.8
            gently	2
            ghost	-1.3
            giddy	-0.6
            gift	1.9
            giggle	1.8
            giggled	1.5
            giggler	0.6
            gigglers	1.4
            giggles	0.8
            gigglier	1
            giggliest	1.7
            giggling	1.5
            gigglingly	1.1
            giggly	1
            giver	1.4
            givers	1.7
            giving	1.4
            glad	2
            gladly	1.4
            glamor	2.1
            glamorise	1.3
            glamorised	1.8
            glamorises	2.1
            glamorising	1.2
            glamorization	1.6
            glamorize	1.7
            glamorized	2.1
            glamorizer	2.4
            glamorizers	1.6
            glamorizes	2.4
            glamorizing	1.8
            glamorous	2.3
            glamorously	2.1
            glamors	1.4
            glamour	2.4
            glamourize	0.8
            glamourless	-1.6
            glamourous	2
            glamours	1.9
            glee	3.2
            gleeful	2.9
            gloom	-2.6
            gloomed	-1.9
            gloomful	-2.1
            gloomier	-1.5
            gloomiest	-1.8
            gloominess	-1.8
            gloominesses	-1
            glooming	-1.8
            glooms	-0.9
            gloomy	-0.6
            gloried	2.4
            glories	2.1
            glorification	2
            glorified	2.3
            glorifier	2.3
            glorifiers	1.6
            glorifies	2.2
            glorify	2.7
            glorifying	2.4
            gloriole	1.5
            glorioles	1.2
            glorious	3.2
            gloriously	2.9
            gloriousness	2.6
            glory	2.5
            glum	-2.1
            gn8	0.6
            god	1.1
            goddam	-2.5
            goddammed	-2.4
            goddamn	-2.1
            goddamned	-1.8
            goddamns	-2.1
            goddams	-1.9
            godsend	2.8
            good	1.9
            goodness	2
            gorgeous	3
            gorgeously	2.3
            gorgeousness	2.9
            gorgeousnesses	2.1
            gossip	-0.7
            gossiped	-1.1
            gossiper	-1.1
            gossipers	-1.1
            gossiping	-1.6
            gossipmonger	-1
            gossipmongers	-1.4
            gossipped	-1.3
            gossipping	-1.8
            gossipries	-0.8
            gossipry	-1.2
            gossips	-1.3
            gossipy	-1.3
            grace	1.8
            graced	0.9
            graceful	2
            gracefuller	2.2
            gracefullest	2.8
            gracefully	2.4
            gracefulness	2.2
            graces	1.6
            gracile	1.7
            graciles	0.6
            gracilis	0.4
            gracility	1.2
            gracing	1.3
            gracioso	1
            gracious	2.6
            graciously	2.3
            graciousness	2.4
            grand	2
            grandee	1.1
            grandees	1.2
            grander	1.7
            grandest	2.4
            grandeur	2.4
            grandeurs	2.1
            grant	1.5
            granted	1
            granting	1.3
            grants	0.9
            grateful	2
            gratefuller	1.8
            gratefully	2.1
            gratefulness	2.2
            graticule	0.1
            graticules	0.2
            gratification	1.6
            gratifications	1.8
            gratified	1.6
            gratifies	1.5
            gratify	1.3
            gratifying	2.3
            gratifyingly	2
            gratin	0.4
            grating	-0.4
            gratingly	-0.2
            gratings	-0.8
            gratins	0.2
            gratis	0.2
            gratitude	2.3
            gratz	2
            grave	-1.6
            graved	-0.9
            gravel	-0.5
            graveled	-0.5
            graveless	-1.3
            graveling	-0.4
            gravelled	-0.9
            gravelling	-0.4
            gravelly	-0.9
            gravels	-0.5
            gravely	-1.5
            graven	-0.9
            graveness	-1.5
            graver	-1.1
            gravers	-1.2
            graves	-1.2
            graveside	-0.8
            gravesides	-1.6
            gravest	-1.3
            gravestone	-0.7
            gravestones	-0.5
            graveyard	-1.2
            graveyards	-1.2
            great	3.1
            greater	1.5
            greatest	3.2
            greed	-1.7
            greedier	-2
            greediest	-2.8
            greedily	-1.9
            greediness	-1.7
            greeds	-1
            greedy	-1.3
            greenwash	-1.8
            greenwashing	-0.4
            greet	1.3
            greeted	1.1
            greeting	1.6
            greetings	1.8
            greets	0.6
            grey	0.2
            grief	-2.2
            grievance	-2.1
            grievances	-1.5
            grievant	-0.8
            grievants	-1.1
            grieve	-1.6
            grieved	-2
            griever	-1.9
            grievers	-0.3
            grieves	-2.1
            grieving	-2.3
            grievous	-2
            grievously	-1.7
            grievousness	-2.7
            grim	-2.7
            grimace	-1
            grimaced	-2
            grimaces	-1.8
            grimacing	-1.4
            grimalkin	-0.9
            grimalkins	-0.9
            grime	-1.5
            grimed	-1.2
            grimes	-1
            grimier	-1.6
            grimiest	-0.7
            grimily	-0.7
            griminess	-1.6
            griming	-0.7
            grimly	-1.3
            grimmer	-1.5
            grimmest	-0.8
            grimness	-0.8
            grimy	-1.8
            grin	2.1
            grinned	1.1
            grinner	1.1
            grinners	1.6
            grinning	1.5
            grins	0.9
            gross	-2.1
            grossed	-0.4
            grosser	-0.3
            grosses	-0.8
            grossest	-2.1
            grossing	-0.3
            grossly	-0.9
            grossness	-1.8
            grossular	-0.3
            grossularite	-0.1
            grossularites	-0.7
            grossulars	-0.3
            grouch	-2.2
            grouched	-0.8
            grouches	-0.9
            grouchier	-2
            grouchiest	-2.3
            grouchily	-1.4
            grouchiness	-2
            grouching	-1.7
            grouchy	-1.9
            growing	0.7
            growth	1.6
            guarantee	1
            guilt	-1.1
            guiltier	-2
            guiltiest	-1.7
            guiltily	-1.1
            guiltiness	-1.8
            guiltless	0.8
            guiltlessly	0.7
            guiltlessness	0.6
            guilts	-1.4
            guilty	-1.8
            gullibility	-1.6
            gullible	-1.5
            gun	-1.4
            h8	-2.7
            ha	1.4
            hacked	-1.7
            haha	2
            hahaha	2.6
            hahas	1.8
            hail	0.3
            hailed	0.9
            hallelujah	3
            handsome	2.2
            handsomely	1.9
            handsomeness	2.4
            handsomer	2
            handsomest	2.6
            hapless	-1.4
            haplessness	-1.4
            happier	2.4
            happiest	3.2
            happily	2.6
            happiness	2.6
            happing	1.1
            happy	2.7
            harass	-2.2
            harassed	-2.5
            harasser	-2.4
            harassers	-2.8
            harasses	-2.5
            harassing	-2.5
            harassment	-2.5
            harassments	-2.6
            hard	-0.4
            hardier	-0.6
            hardship	-1.3
            hardy	1.7
            harm	-2.5
            harmed	-2.1
            harmfully	-2.6
            harmfulness	-2.6
            harming	-2.6
            harmless	1
            harmlessly	1.4
            harmlessness	0.8
            harmonic	1.8
            harmonica	0.6
            harmonically	2.1
            harmonicas	0.1
            harmonicist	0.5
            harmonicists	0.9
            harmonics	1.5
            harmonies	1.3
            harmonious	2
            harmoniously	1.9
            harmoniousness	1.8
            harmonise	1.8
            harmonised	1.3
            harmonising	1.4
            harmonium	0.9
            harmoniums	0.8
            harmonization	1.9
            harmonizations	0.9
            harmonize	1.7
            harmonized	1.6
            harmonizer	1.6
            harmonizers	1.6
            harmonizes	1.5
            harmonizing	1.4
            harmony	1.7
            harms	-2.2
            harried	-1.4
            harsh	-1.9
            harsher	-2.2
            harshest	-2.9
            hate	-2.7
            hated	-3.2
            hateful	-2.2
            hatefully	-2.3
            hatefulness	-3.6
            hater	-1.8
            haters	-2.2
            hates	-1.9
            hating	-2.3
            hatred	-3.2
            haunt	-1.7
            haunted	-2.1
            haunting	-1.1
            haunts	-1
            havoc	-2.9
            healthy	1.7
            heartbreak	-2.7
            heartbreaker	-2.2
            heartbreakers	-2.1
            heartbreaking	-2
            heartbreakingly	-1.8
            heartbreaks	-1.8
            heartbroken	-3.3
            heartfelt	2.5
            heartless	-2.2
            heartlessly	-2.8
            heartlessness	-2.8
            heartwarming	2.1
            heaven	2.3
            heavenlier	3
            heavenliest	2.7
            heavenliness	2.7
            heavenlinesses	2.3
            heavenly	3
            heavens	1.7
            heavenward	1.4
            heavenwards	1.2
            heavyhearted	-2.1
            heh	-0.6
            hell	-3.6
            hellish	-3.2
            help	1.7
            helper	1.4
            helpers	1.1
            helpful	1.8
            helpfully	2.3
            helpfulness	1.9
            helping	1.2
            helpless	-2
            helplessly	-1.4
            helplessness	-2.1
            helplessnesses	-1.7
            helps	1.6
            hero	2.6
            heroes	2.3
            heroic	2.6
            heroical	2.9
            heroically	2.4
            heroicomic	1
            heroicomical	1.1
            heroics	2.4
            heroin	-2.2
            heroine	2.7
            heroines	1.8
            heroinism	-2
            heroism	2.8
            heroisms	2.2
            heroize	2.1
            heroized	2
            heroizes	2.2
            heroizing	1.9
            heron	0.1
            heronries	0.7
            heronry	0.1
            herons	0.5
            heros	1.3
            hesitance	-0.9
            hesitancies	-1
            hesitancy	-0.9
            hesitant	-1
            hesitantly	-1.2
            hesitate	-1.1
            hesitated	-1.3
            hesitater	-1.4
            hesitaters	-1.4
            hesitates	-1.4
            hesitating	-1.4
            hesitatingly	-1.5
            hesitation	-1.1
            hesitations	-1.1
            hid	-0.4
            hide	-0.7
            hides	-0.7
            hiding	-1.2
            highlight	1.4
            hilarious	1.7
            hindrance	-1.7
            hoax	-1.1
            holiday	1.7
            holidays	1.6
            homesick	-0.7
            homesickness	-1.8
            homesicknesses	-1.8
            honest	2.3
            honester	1.9
            honestest	3
            honesties	1.8
            honestly	2
            honesty	2.2
            honor	2.2
            honorability	2.2
            honorable	2.5
            honorableness	2.2
            honorably	2.4
            honoraria	0.6
            honoraries	1.5
            honorarily	1.9
            honorarium	0.7
            honorariums	1
            honorary	1.4
            honored	2.8
            honoree	2.1
            honorees	2.3
            honorer	1.7
            honorers	1.3
            honorific	1.4
            honorifically	2.2
            honorifics	1.7
            honoring	2.3
            honors	2.3
            honour	2.7
            honourable	2.1
            honoured	2.2
            honourer	1.8
            honourers	1.6
            honouring	2.1
            honours	2.2
            hooligan	-1.5
            hooliganism	-2.1
            hooligans	-1.1
            hooray	2.3
            hope	1.9
            hoped	1.6
            hopeful	2.3
            hopefully	1.7
            hopefulness	1.6
            hopeless	-2
            hopelessly	-2.2
            hopelessness	-3.1
            hopes	1.8
            hoping	1.8
            horrendous	-2.8
            horrendously	-1.9
            horrent	-0.9
            horrible	-2.5
            horribleness	-2.4
            horribles	-2.1
            horribly	-2.4
            horrid	-2.5
            horridly	-1.4
            horridness	-2.3
            horridnesses	-3
            horrific	-3.4
            horrifically	-2.9
            horrified	-2.5
            horrifies	-2.9
            horrify	-2.5
            horrifying	-2.7
            horrifyingly	-3.3
            horror	-2.7
            horrors	-2.7
            hostile	-1.6
            hostilely	-2.2
            hostiles	-1.3
            hostilities	-2.1
            hostility	-2.5
            huckster	-0.9
            hug	2.1
            huge	1.3
            huggable	1.6
            hugged	1.7
            hugger	1.6
            huggers	1.8
            hugging	1.8
            hugs	2.2
            humerous	1.4
            humiliate	-2.5
            humiliated	-1.4
            humiliates	-1
            humiliating	-1.2
            humiliatingly	-2.6
            humiliation	-2.7
            humiliations	-2.4
            humor	1.1
            humoral	0.6
            humored	1.2
            humoresque	1.2
            humoresques	0.9
            humoring	2.1
            humorist	1.2
            humoristic	1.5
            humorists	1.3
            humorless	-1.3
            humorlessness	-1.4
            humorous	1.6
            humorously	2.3
            humorousness	2.4
            humors	1.6
            humour	2.1
            humoured	1.1
            humouring	1.7
            humourous	2
            hunger	-1
            hurrah	2.6
            hurrahed	1.9
            hurrahing	2.4
            hurrahs	2.1
            hurray	2.7
            hurrayed	1.8
            hurraying	1.2
            hurrays	2.4
            hurt	-2.4
            hurter	-2.3
            hurters	-1.9
            hurtful	-2.4
            hurtfully	-2.6
            hurtfulness	-1.9
            hurting	-1.7
            hurtle	-0.3
            hurtled	-0.6
            hurtles	-1
            hurtless	0.3
            hurtling	-1.4
            hurts	-2.1
            hypocritical	-2
            hysteria	-1.9
            hysterical	-0.1
            hysterics	-1.8
            ideal	2.4
            idealess	-1.9
            idealise	1.4
            idealised	2.1
            idealises	2
            idealising	0.6
            idealism	1.7
            idealisms	0.8
            idealist	1.6
            idealistic	1.8
            idealistically	1.7
            idealists	0.7
            idealities	1.5
            ideality	1.9
            idealization	1.8
            idealizations	1.4
            idealize	1.2
            idealized	1.8
            idealizer	1.3
            idealizers	1.9
            idealizes	2
            idealizing	1.4
            idealless	-1.7
            ideally	1.8
            idealogues	0.5
            idealogy	0.8
            ideals	0.8
            idiot	-2.3
            idiotic	-2.6
            ignorable	-1
            ignorami	-1.9
            ignoramus	-1.9
            ignoramuses	-2.3
            ignorance	-1.5
            ignorances	-1.2
            ignorant	-1.1
            ignorantly	-1.6
            ignorantness	-1.1
            ignore	-1.5
            ignored	-1.3
            ignorer	-1.3
            ignorers	-0.7
            ignores	-1.1
            ignoring	-1.7
            ill	-1.8
            illegal	-2.6
            illiteracy	-1.9
            illness	-1.7
            illnesses	-2.2
            imbecile	-2.2
            immobilized	-1.2
            immoral	-2
            immoralism	-1.6
            immoralist	-2.1
            immoralists	-1.7
            immoralities	-1.1
            immorality	-0.6
            immorally	-2.1
            immortal	1
            immune	1.2
            impatience	-1.8
            impatiens	-0.2
            impatient	-1.2
            impatiently	-1.7
            imperfect	-1.3
            impersonal	-1.3
            impolite	-1.6
            impolitely	-1.8
            impoliteness	-1.8
            impolitenesses	-2.3
            importance	1.5
            importancies	0.4
            importancy	1.4
            important	0.8
            importantly	1.3
            impose	-1.2
            imposed	-0.3
            imposes	-0.4
            imposing	-0.4
            impotent	-1.1
            impress	1.9
            impressed	2.1
            impresses	2.1
            impressibility	1.2
            impressible	0.8
            impressing	2.5
            impression	0.9
            impressionable	0.2
            impressionism	0.8
            impressionisms	0.5
            impressionist	1
            impressionistic	1.5
            impressionistically	1.6
            impressionists	0.5
            impressions	0.9
            impressive	2.3
            impressively	2
            impressiveness	1.7
            impressment	-0.4
            impressments	0.5
            impressure	0.6
            imprisoned	-2
            improve	1.9
            improved	2.1
            improvement	2
            improvements	1.3
            improver	1.8
            improvers	1.3
            improves	1.8
            improving	1.8
            inability	-1.7
            inaction	-1
            inadequacies	-1.7
            inadequacy	-1.7
            inadequate	-1.7
            inadequately	-1
            inadequateness	-1.7
            inadequatenesses	-1.6
            incapable	-1.6
            incapacitated	-1.9
            incensed	-2
            incentive	1.5
            incentives	1.3
            incompetence	-2.3
            incompetent	-2.1
            inconsiderate	-1.9
            inconvenience	-1.5
            inconvenient	-1.4
            increase	1.3
            increased	1.1
            indecision	-0.8
            indecisions	-1.1
            indecisive	-1
            indecisively	-0.7
            indecisiveness	-1.3
            indecisivenesses	-0.9
            indestructible	0.6
            indifference	-0.2
            indifferent	-0.8
            indignant	-1.8
            indignation	-2.4
            indoctrinate	-1.4
            indoctrinated	-0.4
            indoctrinates	-0.6
            indoctrinating	-0.7
            ineffective	-0.5
            ineffectively	-1.3
            ineffectiveness	-1.3
            ineffectual	-1.2
            ineffectuality	-1.6
            ineffectually	-1.1
            ineffectualness	-1.3
            infatuated	0.2
            infatuation	0.6
            infected	-2.2
            inferior	-1.7
            inferiorities	-1.9
            inferiority	-1.1
            inferiorly	-2
            inferiors	-0.5
            inflamed	-1.4
            influential	1.9
            infringement	-2.1
            infuriate	-2.2
            infuriated	-3
            infuriates	-2.6
            infuriating	-2.4
            inhibin	-0.2
            inhibit	-1.6
            inhibited	-0.4
            inhibiting	-0.4
            inhibition	-1.5
            inhibitions	-0.8
            inhibitive	-1.4
            inhibitor	-0.3
            inhibitors	-1
            inhibitory	-1
            inhibits	-0.9
            injured	-1.7
            injury	-1.8
            injustice	-2.7
            innocence	1.6
            innocency	1.9
            innocent	1.4
            innocenter	0.9
            innocently	1.4
            innocents	1.1
            innovate	2.2
            innovates	2
            innovation	1.6
            innovative	1.9
            inquisition	-1.2
            inquisitive	0.7
            insane	-1.7
            insanity	-2.7
            insecure	-1.8
            insecurely	-1.4
            insecureness	-1.8
            insecurities	-1.8
            insecurity	-1.8
            insensitive	-0.9
            insensitivity	-1.8
            insignificant	-1.4
            insincere	-1.8
            insincerely	-1.9
            insincerity	-1.4
            insipid	-2
            inspiration	2.4
            inspirational	2.3
            inspirationally	2.3
            inspirations	2.1
            inspirator	1.9
            inspirators	1.2
            inspiratory	1.5
            inspire	2.7
            inspired	2.2
            inspirer	2.2
            inspirers	2
            inspires	1.9
            inspiring	1.8
            inspiringly	2.6
            inspirit	1.9
            inspirited	1.3
            inspiriting	1.8
            inspiritingly	2.1
            inspirits	0.8
            insult	-2.3
            insulted	-2.3
            insulter	-2
            insulters	-2
            insulting	-2.2
            insultingly	-2.3
            insults	-1.8
            intact	0.8
            integrity	1.6
            intellect	2
            intellection	0.6
            intellections	0.8
            intellective	1.7
            intellectively	0.8
            intellects	1.8
            intellectual	2.3
            intellectualism	2.2
            intellectualist	2
            intellectualistic	1.3
            intellectualists	0.8
            intellectualities	1.7
            intellectuality	1.7
            intellectualization	1.5
            intellectualize	1.5
            intellectualized	1.2
            intellectualizes	1.8
            intellectualizing	0.8
            intellectually	1.4
            intellectualness	1.5
            intellectuals	1.6
            intelligence	2.1
            intelligencer	1.5
            intelligencers	1.6
            intelligences	1.6
            intelligent	2
            intelligential	1.9
            intelligently	2
            intelligentsia	1.5
            intelligibility	1.5
            intelligible	1.4
            intelligibleness	1.5
            intelligibly	1.2
            intense	0.3
            interest	2
            interested	1.7
            interestedly	1.5
            interesting	1.7
            interestingly	1.7
            interestingness	1.8
            interests	1
            interrogated	-1.6
            interrupt	-1.4
            interrupted	-1.2
            interrupter	-1.1
            interrupters	-1.3
            interruptible	-1.3
            interrupting	-1.2
            interruption	-1.5
            interruptions	-1.7
            interruptive	-1.4
            interruptor	-1.3
            interrupts	-1.3
            intimidate	-0.8
            intimidated	-1.9
            intimidates	-1.3
            intimidating	-1.9
            intimidatingly	-1.1
            intimidation	-1.8
            intimidations	-1.4
            intimidator	-1.6
            intimidators	-1.6
            intimidatory	-1.1
            intricate	0.6
            intrigues	0.9
            invigorate	1.9
            invigorated	0.8
            invigorates	2.1
            invigorating	2.1
            invigoratingly	2
            invigoration	1.5
            invigorations	1.2
            invigorator	1.1
            invigorators	1.2
            invincible	2.2
            invite	0.6
            inviting	1.3
            invulnerable	1.3
            irate	-2.9
            ironic	-0.5
            irony	-0.2
            irrational	-1.4
            irrationalism	-1.5
            irrationalist	-2.1
            irrationalists	-1.5
            irrationalities	-1.5
            irrationality	-1.7
            irrationally	-1.6
            irrationals	-1.1
            irresistible	1.4
            irresolute	-1.4
            irresponsible	-1.9
            irreversible	-0.8
            irritabilities	-1.7
            irritability	-1.4
            irritable	-2.1
            irritableness	-1.7
            irritably	-1.8
            irritant	-2.3
            irritants	-2.1
            irritate	-1.8
            irritated	-2
            irritates	-1.7
            irritating	-2
            irritatingly	-2
            irritation	-2.3
            irritations	-1.5
            irritative	-2
            isolatable	0.2
            isolate	-0.8
            isolated	-1.3
            isolates	-1.3
            isolation	-1.7
            isolationism	0.4
            isolationist	0.7
            isolations	-0.5
            isolator	-0.4
            isolators	-0.4
            itchy	-1.1
            jackass	-1.8
            jackasses	-2.8
            jaded	-1.6
            jailed	-2.2
            jaunty	1.2
            jealous	-2
            jealousies	-2
            jealously	-2
            jealousness	-1.7
            jealousy	-1.3
            jeopardy	-2.1
            jerk	-1.4
            jerked	-0.8
            jerks	-1.1
            jewel	1.5
            jewels	2
            jocular	1.2
            join	1.2
            joke	1.2
            joked	1.3
            joker	0.5
            jokes	1
            jokester	1.5
            jokesters	0.9
            jokey	1.1
            joking	0.9
            jollied	2.4
            jollier	2.4
            jollies	2
            jolliest	2.9
            jollification	2.2
            jollifications	2
            jollify	2.1
            jollily	2.7
            jolliness	2.5
            jollities	1.7
            jollity	1.8
            jolly	2.3
            jollying	2.3
            jovial	1.9
            joy	2.8
            joyance	2.3
            joyed	2.9
            joyful	2.9
            joyfuller	2.4
            joyfully	2.5
            joyfulness	2.7
            joying	2.5
            joyless	-2.5
            joylessly	-1.7
            joylessness	-2.7
            joyous	3.1
            joyously	2.9
            joyousness	2.8
            joypop	-0.2
            joypoppers	-0.1
            joyridden	0.6
            joyride	1.1
            joyrider	0.7
            joyriders	1.3
            joyrides	0.8
            joyriding	0.9
            joyrode	1
            joys	2.2
            joystick	0.7
            joysticks	0.2
            jubilant	3
            jumpy	-1
            justice	2.4
            justifiably	1
            justified	1.7
            keen	1.5
            keened	0.3
            keener	0.5
            keeners	0.6
            keenest	1.9
            keening	-0.7
            keenly	1
            keenness	1.4
            keens	0.1
            kewl	1.3
            kidding	0.4
            kill	-3.7
            killdeer	-1.1
            killdeers	-0.1
            killdees	-0.6
            killed	-3.5
            killer	-3.3
            killers	-3.3
            killick	0.1
            killie	-0.1
            killifish	-0.1
            killifishes	-0.1
            killing	-3.4
            killingly	-2.6
            killings	-3.5
            killjoy	-2.1
            killjoys	-1.7
            killock	-0.3
            killocks	-0.4
            kills	-2.5
            kind	2.4
            kinder	2.2
            kindly	2.2
            kindness	2
            kindnesses	2.3
            kiss	1.8
            kissable	2
            kissably	1.9
            kissed	1.6
            kisser	1.7
            kissers	1.5
            kisses	2.3
            kissing	2.7
            kissy	1.8
            kudos	2.3
            lack	-1.3
            lackadaisical	-1.6
            lag	-1.4
            lagged	-1.2
            lagging	-1.1
            lags	-1.5
            laidback	0.5
            lame	-1.8
            lamebrain	-1.6
            lamebrained	-2.5
            lamebrains	-1.2
            lamedh	0.1
            lamella	-0.1
            lamellae	-0.1
            lamellas	0.1
            lamellibranch	0.2
            lamellibranchs	-0.1
            lamely	-2
            lameness	-0.8
            lament	-2
            lamentable	-1.5
            lamentableness	-1.3
            lamentably	-1.5
            lamentation	-1.4
            lamentations	-1.9
            lamented	-1.4
            lamenter	-1.2
            lamenters	-0.5
            lamenting	-2
            laments	-1.5
            lamer	-1.4
            lames	-1.2
            lamest	-1.5
            landmark	0.3
            laugh	2.6
            laughable	0.2
            laughableness	1.2
            laughably	1.2
            laughed	2
            laugher	1.7
            laughers	1.7
            laughing	2.2
            laughingly	2.3
            laughings	1.9
            laughingstocks	-1.3
            laughs	2.2
            laughter	2.2
            laughters	2.2
            launched	0.5
            lawl	1.4
            lawsuit	-0.9
            lawsuits	-0.6
            lazier	-2.3
            laziest	-2.7
            lazy	-1.5
            leak	-1.4
            leaked	-1.3
            leave	-0.2
            leet	1.3
            legal	0.5
            legally	0.4
            lenient	1.1
            lethargic	-1.2
            lethargy	-1.4
            liabilities	-0.8
            liability	-0.8
            liar	-2.3
            liards	-0.4
            liars	-2.4
            libelous	-2.1
            libertarian	0.9
            libertarianism	0.4
            libertarianisms	0.1
            libertarians	0.1
            liberties	2.3
            libertinage	0.2
            libertine	-0.9
            libertines	0.4
            libertinisms	1.2
            liberty	2.4
            lied	-1.6
            lies	-1.8
            lifesaver	2.8
            lighthearted	1.8
            like	1.5
            likeable	2
            liked	1.8
            likes	1.8
            liking	1.7
            limitation	-1.2
            limited	-0.9
            litigation	-0.8
            litigious	-0.8
            livelier	1.7
            liveliest	2.1
            livelihood	0.8
            livelihoods	0.9
            livelily	1.8
            liveliness	1.6
            livelong	1.7
            lively	1.9
            livid	-2.5
            lmao	2.9
            loathe	-2.2
            loathed	-2.1
            loathes	-1.9
            loathing	-2.7
            lobby	0.1
            lobbying	-0.3
            lol	1.8
            lone	-1.1
            lonelier	-1.4
            loneliest	-2.4
            loneliness	-1.8
            lonelinesses	-1.5
            lonely	-1.5
            loneness	-1.1
            loner	-1.3
            loners	-0.9
            lonesome	-1.5
            lonesomely	-1.3
            lonesomeness	-1.8
            lonesomes	-1.4
            longing	-0.1
            longingly	0.7
            longings	0.4
            loom	-0.9
            loomed	-1.1
            looming	-0.5
            looms	-0.6
            loose	-1.3
            looses	-0.6
            lose	-1.7
            loser	-2.4
            losers	-2.4
            loses	-1.3
            losing	-1.6
            loss	-1.3
            losses	-1.7
            lossy	-1.2
            lost	-1.3
            louse	-1.6
            loused	-1
            louses	-1.3
            lousewort	0.1
            louseworts	-0.6
            lousier	-2.2
            lousiest	-2.6
            lousily	-1.2
            lousiness	-1.7
            lousing	-1.1
            lousy	-2.5
            lovable	3
            love	3.2
            loved	2.9
            lovelies	2.2
            lovely	2.8
            lover	2.8
            loverly	2.8
            lovers	2.4
            loves	2.7
            loving	2.9
            lovingly	3.2
            lovingness	2.7
            low	-1.1
            lowball	-0.8
            lowballed	-1.5
            lowballing	-0.7
            lowballs	-1.2
            lowborn	-0.7
            lowboys	-0.6
            lowbred	-2.6
            lowbrow	-1.9
            lowbrows	-0.6
            lowdown	-0.8
            lowdowns	-0.2
            lowe	0.5
            lowed	-0.8
            lower	-1.2
            lowercase	0.3
            lowercased	-0.2
            lowerclassman	-0.4
            lowered	-0.5
            lowering	-1
            lowermost	-1.4
            lowers	-0.5
            lowery	-1.8
            lowest	-1.6
            lowing	-0.5
            lowish	-0.9
            lowland	-0.1
            lowlander	-0.4
            lowlanders	-0.3
            lowlands	-0.1
            lowlier	-1.7
            lowliest	-1.8
            lowlife	-1.5
            lowlifes	-2.2
            lowlight	-2
            lowlights	-0.3
            lowlihead	-0.3
            lowliness	-1.1
            lowlinesses	-1.2
            lowlives	-2.1
            lowly	-1
            lown	0.9
            lowness	-1.3
            lowrider	-0.2
            lowriders	0.1
            lows	-0.8
            lowse	-0.7
            loyal	2.1
            loyalism	1
            loyalisms	0.9
            loyalist	1.5
            loyalists	1.1
            loyally	2.1
            loyalties	1.9
            loyalty	2.5
            luck	2
            lucked	1.9
            luckie	1.6
            luckier	1.9
            luckiest	2.9
            luckily	2.3
            luckiness	1
            lucking	1.2
            luckless	-1.3
            lucks	1.6
            lucky	1.8
            ludicrous	-1.5
            ludicrously	-0.2
            ludicrousness	-1.9
            lugubrious	-2.1
            lulz	2
            lunatic	-2.2
            lunatics	-1.6
            lurk	-0.8
            lurking	-0.5
            lurks	-0.9
            lying	-2.4
            mad	-2.2
            maddening	-2.2
            madder	-1.2
            maddest	-2.8
            madly	-1.7
            madness	-1.9
            magnific	2.3
            magnifical	2.4
            magnifically	2.4
            magnification	1
            magnifications	1.2
            magnificence	2.4
            magnificences	2.3
            magnificent	2.9
            magnificently	3.4
            magnifico	1.8
            magnificoes	1.4
            mandatory	0.3
            maniac	-2.1
            maniacal	-0.3
            maniacally	-1.7
            maniacs	-1.2
            manipulated	-1.6
            manipulating	-1.5
            manipulation	-1.2
            marvel	1.8
            marvelous	2.9
            marvels	2
            masochism	-1.6
            masochisms	-1.1
            masochist	-1.7
            masochistic	-2.2
            masochistically	-1.6
            masochists	-1.2
            masterpiece	3.1
            masterpieces	2.5
            matter	0.1
            matters	0.1
            mature	1.8
            meaningful	1.3
            meaningless	-1.9
            medal	2.1
            mediocrity	-0.3
            meditative	1.4
            meh	-0.3
            melancholia	-0.5
            melancholiac	-2
            melancholias	-1.6
            melancholic	-0.3
            melancholics	-1
            melancholies	-1.1
            melancholy	-1.9
            menace	-2.2
            menaced	-1.7
            mercy	1.5
            merit	1.8
            merited	1.4
            meriting	1.1
            meritocracy	0.6
            meritocrat	0.4
            meritocrats	1.1
            meritorious	2.1
            meritoriously	1.3
            meritoriousness	1.7
            merits	1.7
            merrier	1.7
            merriest	2.7
            merrily	2.4
            merriment	2.4
            merriments	2
            merriness	2.2
            merry	2.5
            merrymaker	2.2
            merrymakers	1.7
            merrymaking	2.2
            merrymakings	2.4
            merrythought	1.1
            merrythoughts	1.6
            mess	-1.5
            messed	-1.4
            messy	-1.5
            methodical	0.6
            mindless	-1.9
            miracle	2.8
            mirth	2.6
            mirthful	2.7
            mirthfully	2
            misbehave	-1.9
            misbehaved	-1.6
            misbehaves	-1.6
            misbehaving	-1.7
            mischief	-1.5
            mischiefs	-0.8
            miser	-1.8
            miserable	-2.2
            miserableness	-2.8
            miserably	-2.1
            miserere	-0.8
            misericorde	0.1
            misericordes	-0.5
            miseries	-2.7
            miserliness	-2.6
            miserly	-1.4
            misers	-1.5
            misery	-2.7
            misgiving	-1.4
            misinformation	-1.3
            misinformed	-1.6
            misinterpreted	-1.3
            misleading	-1.7
            misread	-1.1
            misreporting	-1.5
            misrepresentation	-2
            miss	-0.6
            missed	-1.2
            misses	-0.9
            missing	-1.2
            mistakable	-0.8
            mistake	-1.4
            mistaken	-1.5
            mistakenly	-1.2
            mistaker	-1.6
            mistakers	-1.6
            mistakes	-1.5
            mistaking	-1.1
            misunderstand	-1.5
            misunderstanding	-1.8
            misunderstands	-1.3
            misunderstood	-1.4
            mlm	-1.4
            mmk	0.6
            moan	-0.6
            moaned	-0.4
            moaning	-0.4
            moans	-0.6
            mock	-1.8
            mocked	-1.3
            mocker	-0.8
            mockeries	-1.6
            mockers	-1.3
            mockery	-1.3
            mocking	-1.7
            mocks	-2
            molest	-2.1
            molestation	-1.9
            molestations	-2.9
            molested	-1.9
            molester	-2.3
            molesters	-2.2
            molesting	-2.8
            molests	-3.1
            mongering	-0.8
            monopolize	-0.8
            monopolized	-0.9
            monopolizes	-1.1
            monopolizing	-0.5
            mooch	-1.7
            mooched	-1.4
            moocher	-1.5
            moochers	-1.9
            mooches	-1.4
            mooching	-1.7
            moodier	-1.1
            moodiest	-2.1
            moodily	-1.3
            moodiness	-1.4
            moodinesses	-1.4
            moody	-1.5
            mope	-1.9
            moping	-1
            moron	-2.2
            moronic	-2.7
            moronically	-1.4
            moronity	-1.1
            morons	-1.3
            motherfucker	-3.6
            motherfucking	-2.8
            motivate	1.6
            motivated	2
            motivating	2.2
            motivation	1.4
            mourn	-1.8
            mourned	-1.3
            mourner	-1.6
            mourners	-1.8
            mournful	-1.6
            mournfuller	-1.9
            mournfully	-1.7
            mournfulness	-1.8
            mourning	-1.9
            mourningly	-2.3
            mourns	-2.4
            muah	2.3
            mumpish	-1.4
            murder	-3.7
            murdered	-3.4
            murderee	-3.2
            murderees	-3.1
            murderer	-3.6
            murderers	-3.3
            murderess	-2.2
            murderesses	-2.6
            murdering	-3.3
            murderous	-3.2
            murderously	-3.1
            murderousness	-2.9
            murders	-3
            n00b	-1.6
            nag	-1.5
            nagana	-1.7
            nagged	-1.7
            nagger	-1.8
            naggers	-1.5
            naggier	-1.4
            naggiest	-2.4
            nagging	-1.7
            naggingly	-0.9
            naggy	-1.7
            nags	-1.1
            nah	-0.4
            naive	-1.1
            nastic	0.2
            nastier	-2.3
            nasties	-2.1
            nastiest	-2.4
            nastily	-1.9
            nastiness	-1.1
            nastinesses	-2.6
            nasturtium	0.4
            nasturtiums	0.1
            nasty	-2.6
            natural	1.5
            neat	2
            neaten	1.2
            neatened	2
            neatening	1.3
            neatens	1.1
            neater	1
            neatest	1.7
            neath	0.2
            neatherd	-0.4
            neatly	1.4
            neatness	1.3
            neats	1.1
            needy	-1.4
            negative	-2.7
            negativity	-2.3
            neglect	-2
            neglected	-2.4
            neglecter	-1.7
            neglecters	-1.5
            neglectful	-2
            neglectfully	-2.1
            neglectfulness	-2
            neglecting	-1.7
            neglects	-2.2
            nerd	-1.2
            nerdier	-0.2
            nerdiest	0.6
            nerdish	-0.1
            nerdy	-0.2
            nerves	-0.4
            nervous	-1.1
            nervously	-0.6
            nervousness	-1.2
            neurotic	-1.4
            neurotically	-1.8
            neuroticism	-0.9
            neurotics	-0.7
            nice	1.8
            nicely	1.9
            niceness	1.6
            nicenesses	2.1
            nicer	1.9
            nicest	2.2
            niceties	1.5
            nicety	1.2
            nifty	1.7
            niggas	-1.4
            nigger	-3.3
            no	-1.2
            noble	2
            noisy	-0.7
            nonsense	-1.7
            noob	-0.2
            nosey	-0.8
            notorious	-1.9
            novel	1.3
            numb	-1.4
            numbat	0.2
            numbed	-0.9
            number	0.3
            numberable	0.6
            numbest	-1
            numbfish	-0.4
            numbfishes	-0.7
            numbing	-1.1
            numbingly	-1.3
            numbles	0.4
            numbly	-1.4
            numbness	-1.1
            numbs	-0.7
            numbskull	-2.3
            numbskulls	-2.2
            nurtural	1.5
            nurturance	1.6
            nurturances	1.3
            nurturant	1.7
            nurture	1.4
            nurtured	1.9
            nurturer	1.9
            nurturers	0.8
            nurtures	1.9
            nurturing	2
            nuts	-1.3
            o.o	-0.8
            o/\o	2.1
            o_0	-0.1
            obliterate	-2.9
            obliterated	-2.1
            obnoxious	-2
            obnoxiously	-2.3
            obnoxiousness	-2.1
            obscene	-2.8
            obsess	-1
            obsessed	-0.7
            obsesses	-1
            obsessing	-1.4
            obsession	-1.4
            obsessional	-1.5
            obsessionally	-1.3
            obsessions	-0.9
            obsessive	-0.9
            obsessively	-0.4
            obsessiveness	-1.2
            obsessives	-0.7
            obsolete	-1.2
            obstacle	-1.5
            obstacles	-1.6
            obstinate	-1.2
            odd	-1.3
            offence	-1.2
            offences	-1.4
            offend	-1.2
            offended	-1
            offender	-1.5
            offenders	-1.5
            offending	-2.3
            offends	-2
            offense	-1
            offenseless	0.7
            offenses	-1.5
            offensive	-2
            offensively	-2.8
            offensiveness	-2.3
            offensives	-0.8
            offline	-0.5
            ok	1.2
            okay	0.9
            okays	2.1
            ominous	-1.4
            once-in-a-lifetime	1.8
            openness	1.4
            opportune	1.7
            opportunely	1.5
            opportuneness	1.2
            opportunism	0.4
            opportunisms	0.2
            opportunist	0.2
            opportunistic	-0.1
            opportunistically	0.9
            opportunists	0.3
            opportunities	1.6
            opportunity	1.8
            oppressed	-2.1
            oppressive	-1.7
            optimal	1.5
            optimality	1.9
            optimally	1.3
            optimisation	1.6
            optimisations	1.8
            optimise	1.9
            optimised	1.7
            optimises	1.6
            optimising	1.7
            optimism	2.5
            optimisms	2
            optimist	2.4
            optimistic	1.3
            optimistically	2.1
            optimists	1.6
            optimization	1.6
            optimizations	0.9
            optimize	2.2
            optimized	2
            optimizer	1.5
            optimizers	2.1
            optimizes	1.8
            optimizing	2
            optionless	-1.7
            original	1.3
            outcry	-2.3
            outgoing	1.2
            outmaneuvered	0.5
            outrage	-2.3
            outraged	-2.5
            outrageous	-2
            outrageously	-1.2
            outrageousness	-1.2
            outrageousnesses	-1.3
            outrages	-2.3
            outraging	-2
            outreach	1.1
            outstanding	3
            overjoyed	2.7
            overload	-1.5
            overlooked	-0.1
            overreact	-1
            overreacted	-1.7
            overreaction	-0.7
            overreacts	-2.2
            oversell	-0.9
            overselling	-0.8
            oversells	0.3
            oversimplification	0.2
            oversimplifies	0.1
            oversimplify	-0.6
            overstatement	-1.1
            overstatements	-0.7
            overweight	-1.5
            overwhelm	-0.7
            overwhelmed	0.2
            overwhelmingly	-0.5
            overwhelms	-0.8
            oxymoron	-0.5
            pain	-2.3
            pained	-1.8
            painful	-1.9
            painfuller	-1.7
            painfully	-2.4
            painfulness	-2.7
            paining	-1.7
            painless	1.2
            painlessly	1.1
            painlessness	0.4
            pains	-1.8
            palatable	1.6
            palatableness	0.8
            palatably	1.1
            panic	-2.3
            panicked	-2
            panicking	-1.9
            panicky	-1.5
            panicle	0.5
            panicled	0.1
            panicles	-0.2
            panics	-1.9
            paniculate	0.1
            panicums	-0.1
            paradise	3.2
            paradox	-0.4
            paranoia	-1
            paranoiac	-1.3
            paranoiacs	-0.7
            paranoias	-1.5
            paranoid	-1
            paranoids	-1.6
            pardon	1.3
            pardoned	0.9
            pardoning	1.7
            pardons	1.2
            parley	-0.4
            partied	1.4
            partier	1.4
            partiers	0.7
            parties	1.7
            party	1.7
            partyer	1.2
            partyers	1.1
            partying	1.6
            passion	2
            passional	1.6
            passionate	2.4
            passionately	2.4
            passionateness	2.3
            passionflower	0.3
            passionflowers	0.4
            passionless	-1.9
            passions	2.2
            passive	0.8
            passively	-0.7
            pathetic	-2.7
            pathetical	-1.2
            pathetically	-1.8
            pay	-0.4
            peace	2.5
            peaceable	1.7
            peaceableness	1.8
            peaceably	2
            peaceful	2.2
            peacefuller	1.9
            peacefullest	3.1
            peacefully	2.4
            peacefulness	2.1
            peacekeeper	1.6
            peacekeepers	1.6
            peacekeeping	2
            peacekeepings	1.6
            peacemaker	2
            peacemakers	2.4
            peacemaking	1.7
            peacenik	0.8
            peaceniks	0.7
            peaces	2.1
            peacetime	2.2
            peacetimes	2.1
            peculiar	0.6
            peculiarities	0.1
            peculiarity	0.6
            peculiarly	-0.4
            penalty	-2
            pensive	0.3
            perfect	2.7
            perfecta	1.4
            perfectas	0.6
            perfected	2.7
            perfecter	1.8
            perfecters	1.4
            perfectest	3.1
            perfectibilities	2.1
            perfectibility	1.8
            perfectible	1.5
            perfecting	2.3
            perfection	2.7
            perfectionism	1.3
            perfectionist	1.5
            perfectionistic	0.7
            perfectionists	0.1
            perfections	2.5
            perfective	1.2
            perfectively	2.1
            perfectiveness	0.9
            perfectives	0.9
            perfectivity	2.2
            perfectly	3.2
            perfectness	3
            perfecto	1.3
            perfects	1.6
            peril	-1.7
            perjury	-1.9
            perpetrator	-2.2
            perpetrators	-1
            perplexed	-1.3
            persecute	-2.1
            persecuted	-1.3
            persecutes	-1.2
            persecuting	-1.5
            perturbed	-1.4
            perverse	-1.8
            perversely	-2.2
            perverseness	-2.1
            perversenesses	-0.5
            perversion	-1.3
            perversions	-1.2
            perversities	-1.1
            perversity	-2.6
            perversive	-2.1
            pervert	-2.3
            perverted	-2.5
            pervertedly	-1.2
            pervertedness	-1.2
            perverter	-1.7
            perverters	-0.6
            perverting	-1
            perverts	-2.8
            pesky	-1.2
            pessimism	-1.5
            pessimisms	-2
            pessimist	-1.5
            pessimistic	-1.5
            pessimistically	-2
            pessimists	-1
            petrifaction	-1.9
            petrifactions	-0.3
            petrification	-0.1
            petrifications	-0.4
            petrified	-2.5
            petrifies	-2.3
            petrify	-1.7
            petrifying	-2.6
            pettier	-0.3
            pettiest	-1.3
            petty	-0.8
            phobia	-1.6
            phobias	-2
            phobic	-1.2
            phobics	-1.3
            picturesque	1.6
            pileup	-1.1
            pique	-1.1
            piqued	0.1
            piss	-1.7
            pissant	-1.5
            pissants	-2.5
            pissed	-3.2
            pisser	-2
            pissers	-1.4
            pisses	-1.4
            pissing	-1.7
            pissoir	-0.8
            piteous	-1.2
            pitiable	-1.1
            pitiableness	-1.1
            pitiably	-1.1
            pitied	-1.3
            pitier	-1.2
            pitiers	-1.3
            pities	-1.2
            pitiful	-2.2
            pitifuller	-1.8
            pitifullest	-1.1
            pitifully	-1.2
            pitifulness	-1.2
            pitiless	-1.8
            pitilessly	-2.1
            pitilessness	-0.5
            pity	-1.2
            pitying	-1.4
            pityingly	-1
            pityriasis	-0.8
            play	1.4
            played	1.4
            playful	1.9
            playfully	1.6
            playfulness	1.2
            playing	0.8
            plays	1
            pleasant	2.3
            pleasanter	1.5
            pleasantest	2.6
            pleasantly	2.1
            pleasantness	2.3
            pleasantnesses	2.3
            pleasantries	1.3
            pleasantry	2
            please	1.3
            pleased	1.9
            pleaser	1.7
            pleasers	1
            pleases	1.7
            pleasing	2.4
            pleasurability	1.9
            pleasurable	2.4
            pleasurableness	2.4
            pleasurably	2.6
            pleasure	2.7
            pleasured	2.3
            pleasureless	-1.6
            pleasures	1.9
            pleasuring	2.8
            poised	1
            poison	-2.5
            poisoned	-2.2
            poisoner	-2.7
            poisoners	-3.1
            poisoning	-2.8
            poisonings	-2.4
            poisonous	-2.7
            poisonously	-2.9
            poisons	-2.7
            poisonwood	-1
            pollute	-2.3
            polluted	-2
            polluter	-1.8
            polluters	-2
            pollutes	-2.2
            poor	-2.1
            poorer	-1.5
            poorest	-2.5
            popular	1.8
            popularise	1.6
            popularised	1.1
            popularises	0.5
            popularising	1.2
            popularities	1.6
            popularity	2.1
            popularization	1.3
            popularizations	0.9
            popularize	1.3
            popularized	1.9
            popularizer	1.8
            popularizers	1
            popularizes	1.4
            popularizing	1.5
            popularly	1.8
            positive	2.6
            positively	2.4
            positiveness	2.3
            positivenesses	2.2
            positiver	2.3
            positives	2.4
            positivest	2.9
            positivism	1.6
            positivisms	1.8
            positivist	2
            positivistic	1.9
            positivists	1.7
            positivities	2.6
            positivity	2.3
            possessive	-0.9
            postpone	-0.9
            postponed	-0.8
            postpones	-1.1
            postponing	-0.5
            poverty	-2.3
            powerful	1.8
            powerless	-2.2
            praise	2.6
            praised	2.2
            praiser	2
            praisers	2
            praises	2.4
            praiseworthily	1.9
            praiseworthiness	2.4
            praiseworthy	2.6
            praising	2.5
            pray	1.3
            praying	1.5
            prays	1.4
            prblm	-1.6
            prblms	-2.3
            precious	2.7
            preciously	2.2
            preciousness	1.9
            prejudice	-2.3
            prejudiced	-1.9
            prejudices	-1.8
            prejudicial	-2.6
            prejudicially	-1.5
            prejudicialness	-2.4
            prejudicing	-1.8
            prepared	0.9
            pressure	-1.2
            pressured	-0.9
            pressureless	1
            pressures	-1.3
            pressuring	-1.4
            pressurise	-0.6
            pressurised	-0.4
            pressurises	-0.8
            pressurising	-0.6
            pressurizations	-0.3
            pressurize	-0.7
            pressurized	0.1
            pressurizer	0.1
            pressurizers	-0.7
            pressurizes	-0.2
            pressurizing	-0.2
            pretend	-0.4
            pretending	0.4
            pretends	-0.4
            prettied	1.6
            prettier	2.1
            pretties	1.7
            prettiest	2.7
            pretty	2.2
            prevent	0.1
            prevented	0.1
            preventing	-0.1
            prevents	0.3
            prick	-1.4
            pricked	-0.6
            pricker	-0.3
            prickers	-0.2
            pricket	-0.5
            prickets	0.3
            pricking	-0.9
            prickle	-1
            prickled	-0.2
            prickles	-0.8
            pricklier	-1.6
            prickliest	-1.4
            prickliness	-0.6
            prickling	-0.8
            prickly	-0.9
            pricks	-0.9
            pricky	-0.6
            pride	1.4
            prison	-2.3
            prisoner	-2.5
            prisoners	-2.3
            privilege	1.5
            privileged	1.9
            privileges	1.6
            privileging	0.7
            prize	2.3
            prized	2.4
            prizefight	-0.1
            prizefighter	1
            prizefighters	-0.1
            prizefighting	0.4
            prizefights	0.3
            prizer	1
            prizers	0.8
            prizes	2
            prizewinner	2.3
            prizewinners	2.4
            prizewinning	3
            proactive	1.8
            problem	-1.7
            problematic	-1.9
            problematical	-1.8
            problematically	-2
            problematics	-1.3
            problems	-1.7
            profit	1.9
            profitabilities	1.1
            profitability	1.1
            profitable	1.9
            profitableness	2.4
            profitably	1.6
            profited	1.3
            profiteer	0.8
            profiteered	-0.5
            profiteering	-0.6
            profiteers	0.5
            profiter	0.7
            profiterole	0.4
            profiteroles	0.5
            profiting	1.6
            profitless	-1.5
            profits	1.9
            profitwise	0.9
            progress	1.8
            prominent	1.3
            promiscuities	-0.8
            promiscuity	-1.8
            promiscuous	-0.3
            promiscuously	-1.5
            promiscuousness	-0.9
            promise	1.3
            promised	1.5
            promisee	0.8
            promisees	1.1
            promiser	1.3
            promisers	1.6
            promises	1.6
            promising	1.7
            promisingly	1.2
            promisor	1
            promisors	0.4
            promissory	0.9
            promote	1.6
            promoted	1.8
            promotes	1.4
            promoting	1.5
            propaganda	-1
            prosecute	-1.7
            prosecuted	-1.6
            prosecutes	-1.8
            prosecution	-2.2
            prospect	1.2
            prospects	1.2
            prosperous	2.1
            protect	1.6
            protected	1.9
            protects	1.3
            protest	-1
            protested	-0.5
            protesters	-0.9
            protesting	-1.8
            protests	-0.9
            proud	2.1
            prouder	2.2
            proudest	2.6
            proudful	1.9
            proudhearted	1.4
            proudly	2.6
            provoke	-1.7
            provoked	-1.1
            provokes	-1.3
            provoking	-0.8
            pseudoscience	-1.2
            puke	-2.4
            puked	-1.8
            pukes	-1.9
            puking	-1.8
            pukka	2.8
            punish	-2.4
            punishabilities	-1.7
            punishability	-1.6
            punishable	-1.9
            punished	-2
            punisher	-1.9
            punishers	-2.6
            punishes	-2.1
            punishing	-2.6
            punishment	-2.2
            punishments	-1.8
            punitive	-2.3
            pushy	-1.1
            puzzled	-0.7
            quaking	-1.5
            questionable	-1.2
            questioned	-0.4
            questioning	-0.4
            racism	-3.1
            racist	-3
            racists	-2.5
            radian	0.4
            radiance	1.4
            radiances	1.1
            radiancies	0.8
            radiancy	1.4
            radians	0.2
            radiant	2.1
            radiantly	1.3
            radiants	1.2
            rage	-2.6
            raged	-2
            ragee	-0.4
            rageful	-2.8
            rages	-2.1
            raging	-2.4
            rainy	-0.3
            rancid	-2.5
            rancidity	-2.6
            rancidly	-2.5
            rancidness	-2.6
            rancidnesses	-1.6
            rant	-1.4
            ranter	-1.2
            ranters	-1.2
            rants	-1.3
            rape	-3.7
            raped	-3.6
            raper	-3.4
            rapers	-3.6
            rapes	-3.5
            rapeseeds	-0.5
            raping	-3.8
            rapist	-3.9
            rapists	-3.3
            rapture	0.6
            raptured	0.9
            raptures	0.7
            rapturous	1.7
            rash	-1.7
            ratified	0.6
            reach	0.1
            reached	0.4
            reaches	0.2
            reaching	0.8
            readiness	1
            ready	1.5
            reassurance	1.5
            reassurances	1.4
            reassure	1.4
            reassured	1.7
            reassures	1.5
            reassuring	1.7
            reassuringly	1.8
            rebel	-0.6
            rebeldom	-1.5
            rebelled	-1
            rebelling	-1.1
            rebellion	-0.5
            rebellions	-1.1
            rebellious	-1.2
            rebelliously	-1.8
            rebelliousness	-1.2
            rebels	-0.8
            recession	-1.8
            reckless	-1.7
            recommend	1.5
            recommended	0.8
            recommends	0.9
            redeemed	1.3
            reek	-2.4
            reeked	-2
            reeker	-1.7
            reekers	-1.5
            reeking	-2
            refuse	-1.2
            refused	-1.2
            refusing	-1.7
            regret	-1.8
            regretful	-1.9
            regretfully	-1.9
            regretfulness	-1.6
            regrets	-1.5
            regrettable	-2.3
            regrettably	-2
            regretted	-1.6
            regretter	-1.6
            regretters	-2
            regretting	-1.7
            reinvigorate	2.3
            reinvigorated	1.9
            reinvigorates	1.8
            reinvigorating	1.7
            reinvigoration	2.2
            reject	-1.7
            rejected	-2.3
            rejectee	-2.3
            rejectees	-1.8
            rejecter	-1.6
            rejecters	-1.8
            rejecting	-2
            rejectingly	-1.7
            rejection	-2.5
            rejections	-2.1
            rejective	-1.8
            rejector	-1.8
            rejects	-2.2
            rejoice	1.9
            rejoiced	2
            rejoices	2.1
            rejoicing	2.8
            relax	1.9
            relaxant	1
            relaxants	0.7
            relaxation	2.4
            relaxations	1
            relaxed	2.2
            relaxedly	1.5
            relaxedness	2
            relaxer	1.6
            relaxers	1.4
            relaxes	1.5
            relaxin	1.7
            relaxing	2.2
            relaxins	1.2
            relentless	0.2
            reliant	0.5
            relief	2.1
            reliefs	1.3
            relievable	1.1
            relieve	1.5
            relieved	1.6
            relievedly	1.4
            reliever	1.5
            relievers	1
            relieves	1.5
            relieving	1.5
            relievo	1.3
            relishing	1.6
            reluctance	-1.4
            reluctancy	-1.6
            reluctant	-1
            reluctantly	-0.4
            remarkable	2.6
            remorse	-1.1
            remorseful	-0.9
            remorsefully	-0.7
            remorsefulness	-0.7
            remorseless	-2.3
            remorselessly	-2
            remorselessness	-2.8
            repetitive	-1
            repress	-1.4
            repressed	-1.3
            represses	-1.3
            repressible	-1.5
            repressing	-1.8
            repression	-1.6
            repressions	-1.7
            repressive	-1.4
            repressively	-1.7
            repressiveness	-1
            repressor	-1.4
            repressors	-2.2
            repressurize	-0.3
            repressurized	0.1
            repressurizes	0.1
            repressurizing	-0.1
            repulse	-2.8
            repulsed	-2.2
            rescue	2.3
            rescued	1.8
            rescues	1.3
            resent	-0.7
            resented	-1.6
            resentence	-1
            resentenced	-0.8
            resentences	-0.6
            resentencing	0.2
            resentful	-2.1
            resentfully	-1.4
            resentfulness	-2
            resenting	-1.2
            resentment	-1.9
            resentments	-1.9
            resents	-1.2
            resign	-1.4
            resignation	-1.2
            resignations	-1.2
            resigned	-1
            resignedly	-0.7
            resignedness	-0.8
            resigner	-1.2
            resigners	-1
            resigning	-0.9
            resigns	-1.3
            resolute	1.1
            resolvable	1
            resolve	1.6
            resolved	0.7
            resolvent	0.7
            resolvents	0.4
            resolver	0.7
            resolvers	1.4
            resolves	0.7
            resolving	1.6
            respect	2.1
            respectabilities	1.8
            respectability	2.4
            respectable	1.9
            respectableness	1.2
            respectably	1.7
            respected	2.1
            respecter	2.1
            respecters	1.6
            respectful	2
            respectfully	1.7
            respectfulness	1.9
            respectfulnesses	1.3
            respecting	2.2
            respective	1.8
            respectively	1.4
            respectiveness	1.1
            respects	1.3
            responsible	1.3
            responsive	1.5
            restful	1.5
            restless	-1.1
            restlessly	-1.4
            restlessness	-1.2
            restore	1.2
            restored	1.4
            restores	1.2
            restoring	1.2
            restrict	-1.6
            restricted	-1.6
            restricting	-1.6
            restriction	-1.1
            restricts	-1.3
            retained	0.1
            retard	-2.4
            retarded	-2.7
            retreat	0.8
            revenge	-2.4
            revenged	-0.9
            revengeful	-2.4
            revengefully	-1.4
            revengefulness	-2.2
            revenger	-2.1
            revengers	-2
            revenges	-1.9
            revered	2.3
            revive	1.4
            revives	1.6
            reward	2.7
            rewardable	2
            rewarded	2.2
            rewarder	1.6
            rewarders	1.9
            rewarding	2.4
            rewardingly	2.4
            rewards	2.1
            rich	2.6
            richened	1.9
            richening	1
            richens	0.8
            richer	2.4
            riches	2.4
            richest	2.4
            richly	1.9
            richness	2.2
            richnesses	2.1
            richweed	0.1
            richweeds	-0.1
            ridicule	-2
            ridiculed	-1.5
            ridiculer	-1.6
            ridiculers	-1.6
            ridicules	-1.8
            ridiculing	-1.8
            ridiculous	-1.5
            ridiculously	-1.4
            ridiculousness	-1.1
            ridiculousnesses	-1.6
            rig	-0.5
            rigged	-1.5
            rigid	-0.5
            rigidification	-1.1
            rigidifications	-0.8
            rigidified	-0.7
            rigidifies	-0.6
            rigidify	-0.3
            rigidities	-0.7
            rigidity	-0.7
            rigidly	-0.7
            rigidness	-0.3
            rigorous	-1.1
            rigorously	-0.4
            riot	-2.6
            riots	-2.3
            risk	-1.1
            risked	-0.9
            risker	-0.8
            riskier	-1.4
            riskiest	-1.5
            riskily	-0.7
            riskiness	-1.3
            riskinesses	-1.6
            risking	-1.3
            riskless	1.3
            risks	-1.1
            risky	-0.8
            rob	-2.6
            robber	-2.6
            robed	-0.7
            robing	-1.5
            robs	-2
            robust	1.4
            roflcopter	2.1
            romance	2.6
            romanced	2.2
            romancer	1.3
            romancers	1.7
            romances	1.3
            romancing	2
            romantic	1.7
            romantically	1.8
            romanticise	1.7
            romanticised	1.7
            romanticises	1.3
            romanticising	2.7
            romanticism	2.2
            romanticisms	2.1
            romanticist	1.9
            romanticists	1.3
            romanticization	1.5
            romanticizations	2
            romanticize	1.8
            romanticized	0.9
            romanticizes	1.8
            romanticizing	1.2
            romantics	1.9
            rotten	-2.3
            rude	-2
            rudely	-2.2
            rudeness	-1.5
            ruder	-2.1
            ruderal	-0.8
            ruderals	-0.4
            rudesby	-2
            rudest	-2.5
            ruin	-2.8
            ruinable	-1.6
            ruinate	-2.8
            ruinated	-1.5
            ruinates	-1.5
            ruinating	-1.5
            ruination	-2.7
            ruinations	-1.6
            ruined	-2.1
            ruiner	-2
            ruing	-1.6
            ruining	-1
            ruinous	-2.7
            ruinously	-2.6
            ruinousness	-1
            ruins	-1.9
            sabotage	-2.4
            sad	-2.1
            sadden	-2.6
            saddened	-2.4
            saddening	-2.2
            saddens	-1.9
            sadder	-2.4
            saddest	-3
            sadly	-1.8
            sadness	-1.9
            safe	1.9
            safecracker	-0.7
            safecrackers	-0.9
            safecracking	-0.9
            safecrackings	-0.7
            safeguard	1.6
            safeguarded	1.5
            safeguarding	1.1
            safeguards	1.4
            safekeeping	1.4
            safelight	1.1
            safelights	0.8
            safely	2.2
            safeness	1.5
            safer	1.8
            safes	0.4
            safest	1.7
            safeties	1.5
            safety	1.8
            safetyman	0.3
            salient	1.1
            sappy	-1
            sarcasm	-0.9
            sarcasms	-0.9
            sarcastic	-1
            sarcastically	-1.1
            satisfaction	1.9
            satisfactions	2.1
            satisfactorily	1.6
            satisfactoriness	1.5
            satisfactory	1.5
            satisfiable	1.9
            satisfied	1.8
            satisfies	1.8
            satisfy	2
            satisfying	2
            satisfyingly	1.9
            savage	-2
            savaged	-2
            savagely	-2.2
            savageness	-2.6
            savagenesses	-0.9
            savageries	-1.9
            savagery	-2.5
            savages	-2.4
            save	2.2
            saved	1.8
            scam	-2.7
            scams	-2.8
            scandal	-1.9
            scandalous	-2.4
            scandals	-2.2
            scapegoat	-1.7
            scapegoats	-1.4
            scare	-2.2
            scarecrow	-0.8
            scarecrows	-0.7
            scared	-1.9
            scaremonger	-2.1
            scaremongers	-2
            scarer	-1.7
            scarers	-1.3
            scares	-1.4
            scarey	-1.7
            scaring	-1.9
            scary	-2.2
            sceptic	-1
            sceptical	-1.2
            scepticism	-0.8
            sceptics	-0.7
            scold	-1.7
            scoop	0.6
            scorn	-1.7
            scornful	-1.8
            scream	-1.7
            screamed	-1.3
            screamers	-1.5
            screaming	-1.6
            screams	-1.2
            screw	-0.4
            screwball	-0.2
            screwballs	-0.3
            screwbean	0.3
            screwdriver	0.3
            screwdrivers	0.1
            screwed	-2.2
            screwed up  -1.5
            screwer	-1.2
            screwers	-0.5
            screwier	-0.6
            screwiest	-2
            screwiness	-0.5
            screwing	-0.9
            screwlike	0.1
            screws	-1
            screwup	-1.7
            screwups	-1
            screwworm	-0.4
            screwworms	-0.1
            screwy	-1.4
            scrumptious	2.1
            scrumptiously	1.5
            scumbag	-3.2
            secure	1.4
            secured	1.7
            securely	1.4
            securement	1.1
            secureness	1.4
            securer	1.5
            securers	0.6
            secures	1.3
            securest	2.6
            securing	1.3
            securities	1.2
            securitization	0.2
            securitizations	0.1
            securitize	0.3
            securitized	1.4
            securitizes	1.6
            securitizing	0.7
            security	1.4
            sedition	-1.8
            seditious	-1.7
            seduced	-1.5
            self-confident	2.5
            selfish	-2.1
            selfishly	-1.4
            selfishness	-1.7
            selfishnesses	-2
            sentence	0.3
            sentenced	-0.1
            sentences	0.2
            sentencing	-0.6
            sentimental	1.3
            sentimentalise	1.2
            sentimentalised	0.8
            sentimentalising	0.4
            sentimentalism	1
            sentimentalisms	0.4
            sentimentalist	0.8
            sentimentalists	0.7
            sentimentalities	0.9
            sentimentality	1.2
            sentimentalization	1.2
            sentimentalizations	0.4
            sentimentalize	0.8
            sentimentalized	1.1
            sentimentalizes	1.1
            sentimentalizing	0.8
            sentimentally	1.9
            serene	2
            serious	-0.3
            seriously	-0.7
            seriousness	-0.2
            severe	-1.6
            severed	-1.5
            severely	-2
            severeness	-1
            severer	-1.6
            severest	-1.5
            sexy	2.4
            shake	-0.7
            shakeable	-0.3
            shakedown	-1.2
            shakedowns	-1.4
            shaken	-0.3
            shakeout	-1.3
            shakeouts	-0.8
            shakers	0.3
            shakeup	-0.6
            shakeups	-0.5
            shakier	-0.9
            shakiest	-1.2
            shakily	-0.7
            shakiness	-0.7
            shaking	-0.7
            shaky	-0.9
            shame	-2.1
            shamed	-2.6
            shamefaced	-2.3
            shamefacedly	-1.9
            shamefacedness	-2
            shamefast	-1
            shameful	-2.2
            shamefully	-1.9
            shamefulness	-2.4
            shamefulnesses	-2.3
            shameless	-1.4
            shamelessly	-1.4
            shamelessness	-1.4
            shamelessnesses	-2
            shames	-1.7
            share	1.2
            shared	1.4
            shares	1.2
            sharing	1.8
            shattered	-2.1
            shit	-2.6
            shitake	-0.3
            shitakes	-1.1
            shithead	-3.1
            shitheads	-2.6
            shits	-2.1
            shittah	0.1
            shitted	-1.7
            shittier	-2.1
            shittiest	-3.4
            shittim	-0.6
            shittimwood	-0.3
            shitting	-1.8
            shitty	-2.6
            shock	-1.6
            shockable	-1
            shocked	-1.3
            shocker	-0.6
            shockers	-1.1
            shocking	-1.7
            shockingly	-0.7
            shockproof	1.3
            shocks	-1.6
            shook	-0.4
            shoot	-1.4
            short-sighted	-1.2
            short-sightedness	-1.1
            shortage	-1
            shortages	-0.6
            shrew	-0.9
            shy	-1
            shyer	-0.8
            shying	-0.9
            shylock	-2.1
            shylocked	-0.7
            shylocking	-1.5
            shylocks	-1.4
            shyly	-0.7
            shyness	-1.3
            shynesses	-1.2
            shyster	-1.6
            shysters	-0.9
            sick	-2.3
            sicken	-1.9
            sickened	-2.5
            sickener	-2.2
            sickeners	-2.2
            sickening	-2.4
            sickeningly	-2.1
            sickens	-2
            sigh	0.1
            significance	1.1
            significant	0.8
            silencing	-0.5
            sillibub	-0.1
            sillier	1
            sillies	0.8
            silliest	0.8
            sillily	-0.1
            sillimanite	0.1
            sillimanites	0.2
            silliness	-0.9
            sillinesses	-1.2
            silly	0.1
            sin	-2.6
            sincere	1.7
            sincerely	2.1
            sincereness	1.8
            sincerer	2
            sincerest	2
            sincerities	1.5
            sinful	-2.6
            singleminded	1.2
            sinister	-2.9
            sins	-2
            skeptic	-0.9
            skeptical	-1.3
            skeptically	-1.2
            skepticism	-1
            skepticisms	-1.2
            skeptics	-0.4
            slam	-1.6
            slash	-1.1
            slashed	-0.9
            slashes	-0.8
            slashing	-1.1
            slavery	-3.8
            sleeplessness	-1.6
            slicker	0.4
            slickest	0.3
            sluggish	-1.7
            slut	-2.8
            sluts	-2.7
            sluttier	-2.7
            sluttiest	-3.1
            sluttish	-2.2
            sluttishly	-2.1
            sluttishness	-2.5
            sluttishnesses	-2
            slutty	-2.3
            smart	1.7
            smartass	-2.1
            smartasses	-1.7
            smarted	0.7
            smarten	1.9
            smartened	1.5
            smartening	1.7
            smartens	1.5
            smarter	2
            smartest	3
            smartie	1.3
            smarties	1.7
            smarting	-0.7
            smartly	1.5
            smartness	2
            smartnesses	1.5
            smarts	1.6
            smartweed	0.2
            smartweeds	0.1
            smarty	1.1
            smear	-1.5
            smilax	0.6
            smilaxes	0.3
            smile	1.5
            smiled	2.5
            smileless	-1.4
            smiler	1.7
            smiles	2.1
            smiley	1.7
            smileys	1.5
            smiling	2
            smilingly	2.3
            smog	-1.2
            smother	-1.8
            smothered	-0.9
            smothering	-1.4
            smothers	-1.9
            smothery	-1.1
            smug	0.8
            smugger	-1
            smuggest	-1.5
            smuggle	-1.6
            smuggled	-1.5
            smuggler	-2.1
            smugglers	-1.4
            smuggles	-1.7
            smuggling	-2.1
            smugly	0.2
            smugness	-1.4
            smugnesses	-1.7
            sneaky	-0.9
            snob	-2
            snobbery	-2
            snobbier	-0.7
            snobbiest	-0.5
            snobbily	-1.6
            snobbish	-0.9
            snobbishly	-1.2
            snobbishness	-1.1
            snobbishnesses	-1.7
            snobbism	-1
            snobbisms	-0.3
            snobby	-1.7
            snobs	-1.4
            snub	-1.8
            snubbed	-2
            snubbing	-0.9
            snubs	-2.1
            sob	-1
            sobbed	-1.9
            sobbing	-1.6
            sobering	-0.8
            sobs	-2.5
            sociabilities	1.2
            sociability	1.1
            sociable	1.9
            sociableness	1.5
            sociably	1.6
            sok	1.3
            solemn	-0.3
            solemnified	-0.5
            solemnifies	-0.5
            solemnify	0.3
            solemnifying	0.1
            solemnities	0.3
            solemnity	-1.1
            solemnization	0.7
            solemnize	0.3
            solemnized	-0.7
            solemnizes	0.6
            solemnizing	-0.6
            solemnly	0.8
            solid	0.6
            solidarity	1.2
            solution	1.3
            solutions	0.7
            solve	0.8
            solved	1.1
            solves	1.1
            solving	1.4
            somber	-1.8
            son-of-a-bitch	-2.7
            soothe	1.5
            soothed	0.5
            soothing	1.3
            sophisticated	2.6
            sore	-1.5
            sorrow	-2.4
            sorrowed	-2.4
            sorrower	-2.3
            sorrowful	-2.2
            sorrowfully	-2.3
            sorrowfulness	-2.5
            sorrowing	-1.7
            sorrows	-1.6
            sorry	-0.3
            soulmate	2.9
            spam	-1.5
            spammer	-2.2
            spammers	-1.6
            spamming	-2.1
            spark	0.9
            sparkle	1.8
            sparkles	1.3
            sparkling	1.2
            special	1.7
            speculative	0.4
            spirit	0.7
            spirited	1.3
            spiritless	-1.3
            spite	-2.4
            spited	-2.4
            spiteful	-1.9
            spitefully	-2.3
            spitefulness	-1.5
            spitefulnesses	-2.3
            spites	-1.4
            splendent	2.7
            splendid	2.8
            splendidly	2.1
            splendidness	2.3
            splendiferous	2.6
            splendiferously	1.9
            splendiferousness	1.7
            splendor	3
            splendorous	2.2
            splendors	2
            splendour	2.2
            splendours	2.2
            splendrous	2.2
            sprightly	2
            squelched	-1
            stab	-2.8
            stabbed	-1.9
            stable	1.2
            stabs	-1.9
            stall	-0.8
            stalled	-0.8
            stalling	-0.8
            stamina	1.2
            stammer	-0.9
            stammered	-0.9
            stammerer	-1.1
            stammerers	-0.8
            stammering	-1
            stammers	-0.8
            stampede	-1.8
            stank	-1.9
            startle	-1.3
            startled	-0.7
            startlement	-0.5
            startlements	0.2
            startler	-0.8
            startlers	-0.5
            startles	-0.5
            startling	0.3
            startlingly	-0.3
            starve	-1.9
            starved	-2.6
            starves	-2.3
            starving	-1.8
            steadfast	1
            steal	-2.2
            stealable	-1.7
            stealer	-1.7
            stealers	-2.2
            stealing	-2.7
            stealings	-1.9
            steals	-2.3
            stealth	-0.3
            stealthier	-0.3
            stealthiest	0.4
            stealthily	0.1
            stealthiness	0.2
            stealths	-0.3
            stealthy	-0.1
            stench	-2.3
            stenches	-1.5
            stenchful	-2.4
            stenchy	-2.3
            stereotype	-1.3
            stereotyped	-1.2
            stifled	-1.4
            stimulate	0.9
            stimulated	0.9
            stimulates	1
            stimulating	1.9
            stingy	-1.6
            stink	-1.7
            stinkard	-2.3
            stinkards	-1
            stinkbug	-0.2
            stinkbugs	-1
            stinker	-1.5
            stinkers	-1.2
            stinkhorn	-0.2
            stinkhorns	-0.8
            stinkier	-1.5
            stinkiest	-2.1
            stinking	-2.4
            stinkingly	-1.3
            stinko	-1.5
            stinkpot	-2.5
            stinkpots	-0.7
            stinks	-1
            stinkweed	-0.4
            stinkwood	-0.1
            stinky	-1.5
            stolen	-2.2
            stop	-1.2
            stopped	-0.9
            stopping	-0.6
            stops	-0.6
            stout	0.7
            straight	0.9
            strain	-0.2
            strained	-1.7
            strainer	-0.8
            strainers	-0.3
            straining	-1.3
            strains	-1.2
            strange	-0.8
            strangely	-1.2
            strangled	-2.5
            strength	2.2
            strengthen	1.3
            strengthened	1.8
            strengthener	1.8
            strengtheners	1.4
            strengthening	2.2
            strengthens	2
            strengths	1.7
            stress	-1.8
            stressed	-1.4
            stresses	-2
            stressful	-2.3
            stressfully	-2.6
            stressing	-1.5
            stressless	1.6
            stresslessness	1.6
            stressor	-1.8
            stressors	-2.1
            stricken	-2.3
            strike	-0.5
            strikers	-0.6
            strikes	-1.5
            strong	2.3
            strongbox	0.7
            strongboxes	0.3
            stronger	1.6
            strongest	1.9
            stronghold	0.5
            strongholds	1
            strongish	1.7
            strongly	1.1
            strongman	0.7
            strongmen	0.5
            strongyl	0.6
            strongyles	0.2
            strongyloidosis	-0.8
            strongyls	0.1
            struck	-1
            struggle	-1.3
            struggled	-1.4
            struggler	-1.1
            strugglers	-1.4
            struggles	-1.5
            struggling	-1.8
            stubborn	-1.7
            stubborner	-1.5
            stubbornest	-0.6
            stubbornly	-1.4
            stubbornness	-1.1
            stubbornnesses	-1.5
            stuck	-1
            stunk	-1.6
            stunned	-0.4
            stunning	1.6
            stuns	0.1
            stupid	-2.4
            stupider	-2.5
            stupidest	-2.4
            stupidities	-2
            stupidity	-1.9
            stupidly	-2
            stupidness	-1.7
            stupidnesses	-2.6
            stupids	-2.3
            stutter	-1
            stuttered	-0.9
            stutterer	-1
            stutterers	-1.1
            stuttering	-1.3
            stutters	-1
            suave	2
            submissive	-1.3
            submissively	-1
            submissiveness	-0.7
            substantial	0.8
            subversive	-0.9
            succeed	2.2
            succeeded	1.8
            succeeder	1.2
            succeeders	1.3
            succeeding	2.2
            succeeds	2.2
            success	2.7
            successes	2.6
            successful	2.8
            successfully	2.2
            successfulness	2.7
            succession	0.8
            successional	0.9
            successionally	1.1
            successions	0.1
            successive	1.1
            successively	0.9
            successiveness	1
            successor	0.9
            successors	1.1
            suck	-1.9
            sucked	-2
            sucker	-2.4
            suckered	-2
            suckering	-2.1
            suckers	-2.3
            sucks	-1.5
            sucky	-1.9
            suffer	-2.5
            suffered	-2.2
            sufferer	-2
            sufferers	-2.4
            suffering	-2.1
            suffers	-2.1
            suicidal	-3.5
            suicide	-3.5
            suing	-1.1
            sulking	-1.5
            sulky	-0.8
            sullen	-1.7
            sunnier	2.3
            sunniest	2.4
            sunny	1.8
            sunshine	2.2
            sunshiny	1.9
            super	2.9
            superb	3.1
            superior	2.5
            superiorities	0.8
            superiority	1.4
            superiorly	2.2
            superiors	1
            support	1.7
            supported	1.3
            supporter	1.1
            supporters	1.9
            supporting	1.9
            supportive	1.2
            supportiveness	1.5
            supports	1.5
            supremacies	0.8
            supremacist	0.5
            supremacists	-1
            supremacy	0.2
            suprematists	0.4
            supreme	2.6
            supremely	2.7
            supremeness	2.3
            supremer	2.3
            supremest	2.2
            supremo	1.9
            supremos	1.3
            sure	1.3
            surefire	1
            surefooted	1.9
            surefootedly	1.6
            surefootedness	1.5
            surely	1.9
            sureness	2
            surer	1.2
            surest	1.3
            sureties	1.3
            surety	1
            suretyship	-0.1
            suretyships	0.4
            surprisal	1.5
            surprisals	0.7
            surprise	1.1
            surprised	0.9
            surpriser	0.6
            surprisers	0.3
            surprises	0.9
            surprising	1.1
            surprisingly	1.2
            survived	2.3
            surviving	1.2
            survivor	1.5
            suspect	-1.2
            suspected	-0.9
            suspecting	-0.7
            suspects	-1.4
            suspend	-1.3
            suspended	-2.1
            suspicion	-1.6
            suspicions	-1.5
            suspicious	-1.5
            suspiciously	-1.7
            suspiciousness	-1.2
            sux	-1.5
            swear	-0.2
            swearing	-1
            swears	0.2
            sweet	2
            sweet<3	3
            sweetheart	3.3
            sweethearts	2.8
            sweetie	2.2
            sweeties	2.1
            sweetly	2.1
            sweetness	2.2
            sweets	2.2
            swift	0.8
            swiftly	1.2
            swindle	-2.4
            swindles	-1.5
            swindling	-2
            sympathetic	2.3
            sympathy	1.5
            talent	1.8
            talented	2.3
            talentless	-1.6
            talents	2
            tantrum	-1.8
            tantrums	-1.5
            tard	-2.5
            tears	-0.9
            teas	0.3
            tease	-1.3
            teased	-1.2
            teasel	-0.1
            teaseled	-0.8
            teaseler	-0.8
            teaselers	-1.2
            teaseling	-0.4
            teaselled	-0.4
            teaselling	-0.2
            teasels	-0.1
            teaser	-1
            teasers	-0.7
            teases	-1.2
            teashops	0.2
            teasing	-0.3
            teasingly	-0.4
            teaspoon	0.2
            teaspoonful	0.2
            teaspoonfuls	0.4
            teaspoons	0.5
            teaspoonsful	0.3
            temper	-1.8
            tempers	-1.3
            tendered	0.5
            tenderer	0.6
            tenderers	1.2
            tenderest	1.4
            tenderfeet	-0.4
            tenderfoot	-0.1
            tenderfoots	-0.5
            tenderhearted	1.5
            tenderheartedly	2.7
            tenderheartedness	0.7
            tenderheartednesses	2.8
            tendering	0.6
            tenderization	0.2
            tenderize	0.1
            tenderized	0.1
            tenderizer	0.4
            tenderizes	0.3
            tenderizing	0.3
            tenderloin	-0.2
            tenderloins	0.4
            tenderly	1.8
            tenderness	1.8
            tendernesses	0.9
            tenderometer	0.2
            tenderometers	0.2
            tenders	0.6
            tense	-1.4
            tensed	-1
            tensely	-1.2
            tenseness	-1.5
            tenser	-1.5
            tenses	-0.9
            tensest	-1.2
            tensing	-1
            tension	-1.3
            tensional	-0.8
            tensioned	-0.4
            tensioner	-1.6
            tensioners	-0.9
            tensioning	-1.4
            tensionless	0.6
            tensions	-1.7
            terrible	-2.1
            terribleness	-1.9
            terriblenesses	-2.6
            terribly	-2.6
            terrific	2.1
            terrifically	1.7
            terrified	-3
            terrifies	-2.6
            terrify	-2.3
            terrifying	-2.7
            terror	-2.4
            terrorise	-3.1
            terrorised	-3.3
            terrorises	-3.3
            terrorising	-3
            terrorism	-3.6
            terrorisms	-3.2
            terrorist	-3.7
            terroristic	-3.3
            terrorists	-3.1
            terrorization	-2.7
            terrorize	-3.3
            terrorized	-3.1
            terrorizes	-3.1
            terrorizing	-3
            terrorless	0.9
            terrors	-2.6
            thank	1.5
            thanked	1.9
            thankful	2.7
            thankfuller	1.9
            thankfullest	2
            thankfully	1.8
            thankfulness	2.1
            thanks	1.9
            thief	-2.4
            thieve	-2.2
            thieved	-1.4
            thieveries	-2.1
            thievery	-2
            thieves	-2.3
            thorny	-1.1
            thoughtful	1.6
            thoughtfully	1.7
            thoughtfulness	1.9
            thoughtless	-2
            threat	-2.4
            threaten	-1.6
            threatened	-2
            threatener	-1.4
            threateners	-1.8
            threatening	-2.4
            threateningly	-2.2
            threatens	-1.6
            threating	-2
            threats	-1.8
            thrill	1.5
            thrilled	1.9
            thriller	0.4
            thrillers	0.1
            thrilling	2.1
            thrillingly	2
            thrills	1.5
            thwarted	-0.1
            thwarting	-0.7
            thwarts	-0.4
            ticked	-1.8
            timid	-1
            timider	-1
            timidest	-0.9
            timidities	-0.7
            timidity	-1.3
            timidly	-0.7
            timidness	-1
            timorous	-0.8
            tired	-1.9
            tits	-0.9
            tolerance	1.2
            tolerances	0.3
            tolerant	1.1
            tolerantly	0.4
            toothless	-1.4
            top	0.8
            tops	2.3
            torn	-1
            torture	-2.9
            tortured	-2.6
            torturer	-2.3
            torturers	-3.5
            tortures	-2.5
            torturing	-3
            torturous	-2.7
            torturously	-2.2
            totalitarian	-2.1
            totalitarianism	-2.7
            tough	-0.5
            toughed	0.7
            toughen	0.1
            toughened	0.1
            toughening	0.9
            toughens	-0.2
            tougher	0.7
            toughest	-0.3
            toughie	-0.7
            toughies	-0.6
            toughing	-0.5
            toughish	-1
            toughly	-1.1
            toughness	-0.2
            toughnesses	0.3
            toughs	-0.8
            toughy	-0.5
            tout	-0.5
            touted	-0.2
            touting	-0.7
            touts	-0.1
            tragedian	-0.5
            tragedians	-1
            tragedienne	-0.4
            tragediennes	-1.4
            tragedies	-1.9
            tragedy	-3.4
            tragic	-2
            tragical	-2.4
            tragically	-2.7
            tragicomedy	0.2
            tragicomic	-0.2
            tragics	-2.2
            tranquil	0.2
            tranquiler	1.9
            tranquilest	1.6
            tranquilities	1.5
            tranquility	1.8
            tranquilize	0.3
            tranquilized	-0.2
            tranquilizer	-0.1
            tranquilizers	-0.4
            tranquilizes	-0.1
            tranquilizing	-0.5
            tranquillest	0.8
            tranquillities	0.5
            tranquillity	1.8
            tranquillized	-0.2
            tranquillizer	-0.1
            tranquillizers	-0.2
            tranquillizes	0.1
            tranquillizing	0.8
            tranquilly	1.2
            tranquilness	1.5
            trap	-1.3
            trapped	-2.4
            trauma	-1.8
            traumas	-2.2
            traumata	-1.7
            traumatic	-2.7
            traumatically	-2.8
            traumatise	-2.8
            traumatised	-2.4
            traumatises	-2.2
            traumatising	-1.9
            traumatism	-2.4
            traumatization	-3
            traumatizations	-2.2
            traumatize	-2.4
            traumatized	-1.7
            traumatizes	-1.4
            traumatizing	-2.3
            travesty	-2.7
            treason	-1.9
            treasonous	-2.7
            treasurable	2.5
            treasure	1.2
            treasured	2.6
            treasurer	0.5
            treasurers	0.4
            treasurership	0.4
            treasurerships	1.2
            treasures	1.8
            treasuries	0.9
            treasuring	2.1
            treasury	0.8
            treat	1.7
            tremble	-1.1
            trembled	-1.1
            trembler	-0.6
            tremblers	-1
            trembles	-0.1
            trembling	-1.5
            trembly	-1.2
            tremulous	-1
            trick	-0.2
            tricked	-0.6
            tricker	-0.9
            trickeries	-1.2
            trickers	-1.4
            trickery	-1.1
            trickie	-0.4
            trickier	-0.7
            trickiest	-1.2
            trickily	-0.8
            trickiness	-1.2
            trickinesses	-0.4
            tricking	0.1
            trickish	-1
            trickishly	-0.7
            trickishness	-0.4
            trickled	0.1
            trickledown	-0.7
            trickles	0.2
            trickling	-0.2
            trickly	-0.3
            tricks	-0.5
            tricksier	-0.5
            tricksiness	-1
            trickster	-0.9
            tricksters	-1.3
            tricksy	-0.8
            tricky	-0.6
            trite	-0.8
            triumph	2.1
            triumphal	2
            triumphalisms	1.9
            triumphalist	0.5
            triumphalists	0.9
            triumphant	2.4
            triumphantly	2.3
            triumphed	2.2
            triumphing	2.3
            triumphs	2
            trivial	-0.1
            trivialise	-0.8
            trivialised	-0.8
            trivialises	-1.1
            trivialising	-1.4
            trivialities	-1
            triviality	-0.5
            trivialization	-0.9
            trivializations	-0.7
            trivialize	-1.1
            trivialized	-0.6
            trivializes	-1
            trivializing	-0.6
            trivially	0.4
            trivium	-0.3
            trouble	-1.7
            troubled	-2
            troublemaker	-2
            troublemakers	-2.2
            troublemaking	-1.8
            troubler	-1.4
            troublers	-1.9
            troubles	-2
            troubleshoot	0.8
            troubleshooter	1
            troubleshooters	0.8
            troubleshooting	0.7
            troubleshoots	0.5
            troublesome	-2.3
            troublesomely	-1.8
            troublesomeness	-1.9
            troubling	-2.5
            troublous	-2.1
            troublously	-2.1
            trueness	2.1
            truer	1.5
            truest	1.9
            truly	1.9
            trust	2.3
            trustability	2.1
            trustable	2.3
            trustbuster	-0.5
            trusted	2.1
            trustee	1
            trustees	0.3
            trusteeship	0.5
            trusteeships	0.6
            truster	1.9
            trustful	2.1
            trustfully	1.5
            trustfulness	2.1
            trustier	1.3
            trusties	1
            trustiest	2.2
            trustily	1.6
            trustiness	1.6
            trusting	1.7
            trustingly	1.6
            trustingness	1.6
            trustless	-2.3
            trustor	0.4
            trustors	1.2
            trusts	2.1
            trustworthily	2.3
            trustworthiness	1.8
            trustworthy	2.6
            trusty	2.2
            truth	1.3
            truthful	2
            truthfully	1.9
            truthfulness	1.7
            truths	1.8
            tumor	-1.6
            turmoil	-1.5
            twat	-3.4
            ugh	-1.8
            uglier	-2.2
            uglies	-2
            ugliest	-2.8
            uglification	-2.2
            uglified	-1.5
            uglifies	-1.8
            uglify	-2.1
            uglifying	-2.2
            uglily	-2.1
            ugliness	-2.7
            uglinesses	-2.5
            ugly	-2.3
            unacceptable	-2
            unappreciated	-1.7
            unapproved	-1.4
            unattractive	-1.9
            unaware	-0.8
            unbelievable	0.8
            unbelieving	-0.8
            unbiased	-0.1
            uncertain	-1.2
            uncertainly	-1.4
            uncertainness	-1.3
            uncertainties	-1.4
            uncertainty	-1.4
            unclear	-1
            uncomfortable	-1.6
            uncomfortably	-1.7
            uncompelling	-0.9
            unconcerned	-0.9
            unconfirmed	-0.5
            uncontrollability	-1.7
            uncontrollable	-1.5
            uncontrollably	-1.5
            uncontrolled	-1
            unconvinced	-1.6
            uncredited	-1
            undecided	-0.9
            underestimate	-1.2
            underestimated	-1.1
            underestimates	-1.1
            undermine	-1.2
            undermined	-1.5
            undermines	-1.4
            undermining	-1.5
            undeserving	-1.9
            undesirable	-1.9
            unease	-1.7
            uneasier	-1.4
            uneasiest	-2.1
            uneasily	-1.4
            uneasiness	-1.6
            uneasinesses	-1.8
            uneasy	-1.6
            unemployment	-1.9
            unequal	-1.4
            unequaled	0.5
            unethical	-2.3
            unfair	-2.1
            unfocused	-1.7
            unfortunate	-2
            unfortunately	-1.4
            unfortunates	-1.9
            unfriendly	-1.5
            unfulfilled	-1.8
            ungrateful	-2
            ungratefully	-1.8
            ungratefulness	-1.6
            unhappier	-2.4
            unhappiest	-2.5
            unhappily	-1.9
            unhappiness	-2.4
            unhappinesses	-2.2
            unhappy	-1.8
            unhealthy	-2.4
            unified	1.6
            unimportant	-1.3
            unimpressed	-1.4
            unimpressive	-1.4
            unintelligent	-2
            uninvolved	-2.2
            uninvolving	-2
            united	1.8
            unjust	-2.3
            unkind	-1.6
            unlovable	-2.7
            unloved	-1.9
            unlovelier	-1.9
            unloveliest	-1.9
            unloveliness	-2
            unlovely	-2.1
            unloving	-2.3
            unmatched	-0.3
            unmotivated	-1.4
            unpleasant	-2.1
            unprofessional	-2.3
            unprotected	-1.5
            unresearched	-1.1
            unsatisfied	-1.7
            unsavory	-1.9
            unsecured	-1.6
            unsettled	-1.3
            unsophisticated	-1.2
            unstable	-1.5
            unstoppable	-0.8
            unsuccessful	-1.5
            unsuccessfully	-1.7
            unsupported	-1.7
            unsure	-1
            unsurely	-1.3
            untarnished	1.6
            unwanted	-0.9
            unwelcome	-1.7
            unworthy	-2
            upset	-1.6
            upsets	-1.5
            upsetter	-1.9
            upsetters	-2
            upsetting	-2.1
            uptight	-1.6
            uptightness	-1.2
            urgent	0.8
            useful	1.9
            usefully	1.8
            usefulness	1.2
            useless	-1.8
            uselessly	-1.5
            uselessness	-1.6
            v.v	-2.9
            vague	-0.4
            vain	-1.8
            validate	1.5
            validated	0.9
            validates	1.4
            validating	1.4
            valuable	2.1
            valuableness	1.7
            valuables	2.1
            valuably	2.3
            value	1.4
            valued	1.9
            values	1.7
            valuing	1.4
            vanity	-0.9
            verdict	0.6
            verdicts	0.3
            vested	0.6
            vexation	-1.9
            vexing	-2
            vibrant	2.4
            vicious	-1.5
            viciously	-1.3
            viciousness	-2.4
            viciousnesses	-0.6
            victim	-1.1
            victimhood	-2
            victimhoods	-0.9
            victimise	-1.1
            victimised	-1.5
            victimises	-1.2
            victimising	-2.5
            victimization	-2.3
            victimizations	-1.5
            victimize	-2.5
            victimized	-1.8
            victimizer	-1.8
            victimizers	-1.6
            victimizes	-1.5
            victimizing	-2.6
            victimless	0.6
            victimologies	-0.6
            victimologist	-0.5
            victimologists	-0.4
            victimology	0.3
            victims	-1.3
            vigilant	0.7
            vigor	1.1
            vigorish	-0.4
            vigorishes	0.4
            vigoroso	1.5
            vigorously	0.5
            vigorousness	0.4
            vigors	1
            vigour	0.9
            vigours	0.4
            vile	-3.1
            villain	-2.6
            villainess	-2.9
            villainesses	-2
            villainies	-2.3
            villainous	-2
            villainously	-2.9
            villainousness	-2.7
            villains	-3.4
            villainy	-2.6
            vindicate	0.3
            vindicated	1.8
            vindicates	1.6
            vindicating	-1.1
            violate	-2.2
            violated	-2.4
            violater	-2.6
            violaters	-2.4
            violates	-2.3
            violating	-2.5
            violation	-2.2
            violations	-2.4
            violative	-2.4
            violator	-2.4
            violators	-1.9
            violence	-3.1
            violent	-2.9
            violently	-2.8
            virtue	1.8
            virtueless	-1.4
            virtues	1.5
            virtuosa	1.7
            virtuosas	1.8
            virtuose	1
            virtuosi	0.9
            virtuosic	2.2
            virtuosity	2.1
            virtuoso	2
            virtuosos	1.8
            virtuous	2.4
            virtuously	1.8
            virtuousness	2
            virulent	-2.7
            vision	1
            visionary	2.4
            visioning	1.1
            visions	0.9
            vital	1.2
            vitalise	1.1
            vitalised	0.6
            vitalises	1.1
            vitalising	2.1
            vitalism	0.2
            vitalist	0.3
            vitalists	0.3
            vitalities	1.2
            vitality	1.3
            vitalization	1.6
            vitalizations	0.8
            vitalize	1.6
            vitalized	1.5
            vitalizes	1.4
            vitalizing	1.3
            vitally	1.1
            vitals	1.1
            vitamin	1.2
            vitriolic	-2.1
            vivacious	1.8
            vociferous	-0.8
            vulnerabilities	-0.6
            vulnerability	-0.9
            vulnerable	-0.9
            vulnerableness	-1.1
            vulnerably	-1.2
            vulture	-2
            vultures	-1.3
            w00t	2.2
            walkout	-1.3
            walkouts	-0.7
            wanker	-2.5
            want	0.3
            war	-2.9
            warfare	-1.2
            warfares	-1.8
            warm	0.9
            warmblooded	0.2
            warmed	1.1
            warmer	1.2
            warmers	1
            warmest	1.7
            warmhearted	1.8
            warmheartedness	2.7
            warming	0.6
            warmish	1.4
            warmly	1.7
            warmness	1.5
            warmonger	-2.9
            warmongering	-2.5
            warmongers	-2.8
            warmouth	0.4
            warmouths	-0.8
            warms	1.1
            warmth	2
            warmup	0.4
            warmups	0.8
            warn	-0.4
            warned	-1.1
            warning	-1.4
            warnings	-1.2
            warns	-0.4
            warred	-2.4
            warring	-1.9
            wars	-2.6
            warsaw	-0.1
            warsaws	-0.2
            warship	-0.7
            warships	-0.5
            warstle	0.1
            waste	-1.8
            wasted	-2.2
            wasting	-1.7
            wavering	-0.6
            weak	-1.9
            weaken	-1.8
            weakened	-1.3
            weakener	-1.6
            weakeners	-1.3
            weakening	-1.3
            weakens	-1.3
            weaker	-1.9
            weakest	-2.3
            weakfish	-0.2
            weakfishes	-0.6
            weakhearted	-1.6
            weakish	-1.2
            weaklier	-1.5
            weakliest	-2.1
            weakling	-1.3
            weaklings	-1.4
            weakly	-1.8
            weakness	-1.8
            weaknesses	-1.5
            weakside	-1.1
            wealth	2.2
            wealthier	2.2
            wealthiest	2.2
            wealthily	2
            wealthiness	2.4
            wealthy	1.5
            weapon	-1.2
            weaponed	-1.4
            weaponless	0.1
            weaponry	-0.9
            weapons	-1.9
            weary	-1.1
            weep	-2.7
            weeper	-1.9
            weepers	-1.1
            weepie	-0.4
            weepier	-1.8
            weepies	-1.6
            weepiest	-2.4
            weeping	-1.9
            weepings	-1.9
            weeps	-1.4
            weepy	-1.3
            weird	-0.7
            weirder	-0.5
            weirdest	-0.9
            weirdie	-1.3
            weirdies	-1
            weirdly	-1.2
            weirdness	-0.9
            weirdnesses	-0.7
            weirdo	-1.8
            weirdoes	-1.3
            weirdos	-1.1
            weirds	-0.6
            weirdy	-0.9
            welcome	2
            welcomed	1.4
            welcomely	1.9
            welcomeness	2
            welcomer	1.4
            welcomers	1.9
            welcomes	1.7
            welcoming	1.9
            well	1.1
            welladay	0.3
            wellaway	-0.8
            wellborn	1.8
            welldoer	2.5
            welldoers	1.6
            welled	0.4
            wellhead	0.1
            wellheads	0.5
            wellhole	-0.1
            wellies	0.4
            welling	1.6
            wellness	1.9
            wells	1
            wellsite	0.5
            wellspring	1.5
            wellsprings	1.4
            welly	0.2
            wept	-2
            whimsical	0.3
            whine	-1.5
            whined	-0.9
            whiner	-1.2
            whiners	-0.6
            whines	-1.8
            whiney	-1.3
            whining	-0.9
            whitewash	0.1
            whore	-3.3
            whored	-2.8
            whoredom	-2.1
            whoredoms	-2.4
            whorehouse	-1.1
            whorehouses	-1.9
            whoremaster	-1.9
            whoremasters	-1.5
            whoremonger	-2.6
            whoremongers	-2
            whores	-3
            whoreson	-2.2
            whoresons	-2.5
            wicked	-2.4
            wickeder	-2.2
            wickedest	-2.9
            wickedly	-2.1
            wickedness	-2.1
            wickednesses	-2.2
            widowed	-2.1
            willingness	1.1
            wimp	-1.4
            wimpier	-1
            wimpiest	-0.9
            wimpiness	-1.2
            wimpish	-1.6
            wimpishness	-0.2
            wimple	-0.2
            wimples	-0.3
            wimps	-1
            wimpy	-0.9
            win	2.8
            winnable	1.8
            winned	1.8
            winner	2.8
            winners	2.1
            winning	2.4
            winningly	2.3
            winnings	2.5
            winnow	-0.3
            winnower	-0.1
            winnowers	-0.2
            winnowing	-0.1
            winnows	-0.2
            wins	2.7
            wisdom	2.4
            wise	2.1
            wiseacre	-1.2
            wiseacres	-0.1
            wiseass	-1.8
            wiseasses	-1.5
            wisecrack	-0.1
            wisecracked	-0.5
            wisecracker	-0.1
            wisecrackers	0.1
            wisecracking	-0.6
            wisecracks	-0.3
            wised	1.5
            wiseguys	0.3
            wiselier	0.9
            wiseliest	1.6
            wisely	1.8
            wiseness	1.9
            wisenheimer	-1
            wisenheimers	-1.4
            wisents	0.4
            wiser	1.2
            wises	1.3
            wisest	2.1
            wisewomen	1.3
            wish	1.7
            wishes	0.6
            wishing	0.9
            witch	-1.5
            withdrawal	0.1
            woe	-1.8
            woebegone	-2.6
            woebegoneness	-1.1
            woeful	-1.9
            woefully	-1.7
            woefulness	-2.1
            woes	-1.9
            woesome	-1.2
            won	2.7
            wonderful	2.7
            wonderfully	2.9
            wonderfulness	2.9
            woo	2.1
            woohoo	2.3
            woot	1.8
            worn	-1.2
            worried	-1.2
            worriedly	-2
            worrier	-1.8
            worriers	-1.7
            worries	-1.8
            worriment	-1.5
            worriments	-1.9
            worrisome	-1.7
            worrisomely	-2
            worrisomeness	-1.9
            worrit	-2.1
            worrits	-1.2
            worry	-1.9
            worrying	-1.4
            worrywart	-1.8
            worrywarts	-1.5
            worse	-2.1
            worsen	-2.3
            worsened	-1.9
            worsening	-2
            worsens	-2.1
            worser	-2
            worship	1.2
            worshiped	2.4
            worshiper	1
            worshipers	0.9
            worshipful	0.7
            worshipfully	1.1
            worshipfulness	1.6
            worshiping	1
            worshipless	-0.6
            worshipped	2.7
            worshipper	0.6
            worshippers	0.8
            worshipping	1.6
            worships	1.4
            worst	-3.1
            worth	0.9
            worthless	-1.9
            worthwhile	1.4
            worthy	1.9
            wow	2.8
            wowed	2.6
            wowing	2.5
            wows	2
            wowser	-1.1
            wowsers	1
            wrathful	-2.7
            wreck	-1.9
            wrong	-2.1
            wronged	-1.9
            x-d	2.6
            x-p	1.7
            xd	2.8
            xp	1.6
            yay	2.4
            yeah	1.2
            yearning	0.5
            yeees	1.7
            yep	1.2
            yes	1.7
            youthful	1.3
            yucky	-1.8
            yummy	2.4
            zealot	-1.9
            zealots	-0.8
            zealous	0.5
            {:	1.8
            |-0	-1.2
            |-:	-0.8
            |-:>	-1.6
            |-o	-1.2
            |:	-0.5
            |;-)	2.2
            |=	-0.4
            |^:	-1.1
            |o:	-0.9
            ||-:	-2.3
            }:	-2.1
            }:( -2
            }:)	0.4
            }:-(    -2.1
            }:-)	0.3".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        #endregion
        
        #endregion
    }
}