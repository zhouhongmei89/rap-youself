using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Jie_OfflineTools
{
    using System;
    using System.Text;
    using Microsoft.Tts.Offline;
    using Microsoft.Tts.Offline.Utility;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Microsoft.Tts.ScriptSynthesizer;
    using Microsoft.Tts.ServiceProvider;
    using Microsoft.Tts.ServiceProvider.FeatureExtractor;
    using Microsoft.Tts.Offline.Core;
    using Microsoft.Tts.Offline.Htk;
    using SP = Microsoft.Tts.ServiceProvider;

    class featureExport
    {
        #region Fields


        /// <summary>
        /// Service provider.
        /// </summary>
        private ServiceProvider _serviceProvider;

        private Microsoft.Tts.Offline.Language _lang;

        private List<string> _Phones;
        private List<int> _stress;
        private List<int> _accent;
        private List<string> _Syllable2phoneArrary;
        private List<string> _Syllable2phoneArraryVN;


        private List<string> WordString;
        private List<List<string>> _Syllable2Phones;
        private List<List<bool>> _Syllable2IsVowelPhones;
        private List<List<TtsSyllable>> _phrase2Syllables;
        private List<List<int>> wordPOS2Stress;
        List<List<string>> syllable2phones;
        //private List<string> prepList;
        string[] prepArrary= {"at","in","on","to","for","above","over","below","under","beside","behind","from","since","after","across","through","between","among","about","by","with","except","besides","around"} ;
        /// <summary>
        /// Disposed flag.
        /// </summary>
        private bool _disposed = false;


        private List<uint[]> _durations;
        #endregion

        #region Constructor and destructor

        /// <summary>
        /// Initializes a new instance of the XmlScriptExport class.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="lang">Language.</param>
        public featureExport(ServiceProvider serviceProvider, Microsoft.Tts.Offline.Language lang)
        {
            //// commonConfig can be null.

            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            if (!serviceProvider.Engine.IsHts)
            {
                string message = string.Format(
                    "Only support Hts engine.");
                // throw new NotSupportedException(message);
            }

            _serviceProvider = serviceProvider;
            _lang = lang;

            _serviceProvider.Engine.Processed += OnProcessed;

        }

        /// <summary>
        /// Finalizes an instance of the XmlScriptExport class.
        /// </summary>
        ~featureExport()
        {
            Dispose(false);
        }

        #endregion

        #region Public operations

        /// <summary>
        /// Dispose routine.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Prepare for speak.
        /// </summary>
        /// <param name="currentId">Id.</param>
        /// <param name="text">Text.</param>
        public void PrepareSpeak()
        {
            _Phones = new List<string>();
            _durations = new List<uint[]>();
            _Syllable2Phones = new List<List<string>>();
            _Syllable2IsVowelPhones = new List<List<bool>>();
            _stress = new List<int>();
            _accent = new List<int>();
            _Syllable2phoneArrary = new List<string>();
            _Syllable2phoneArraryVN = new List<string>();
            _phrase2Syllables = new List<List<TtsSyllable>>();
            wordPOS2Stress = new List<List<int>>();
            syllable2phones = new List<List<string>>();
            WordString = new List<string>();
        }

        #endregion

        #region Private operations

        /// <summary>
        /// Dispose managed resource.
        /// </summary>
        private void Close()
        {
            if (_serviceProvider != null)
            {
                _serviceProvider.Engine.Processed -= OnProcessed;
            }
        }

        /// <summary>
        /// Do the dispose work here.
        /// </summary>
        /// <param name="disposing">Whether the functions is called by user's code (true), or by finalizer (false).</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Close();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// The export function.
        /// </summary>
        /// <param name="outAlignmentfile">Argument strings from command line.</param>
        public void ExportDuration(List<int> phonedurations)
        {
            for (int i = 0; i < _durations.Count; ++i)
            {
                int dur = 0;
                for (int j = 0; j < _durations[i].Length; ++j)
                {
                    dur += (int)_durations[i][j];
                }
                phonedurations.Add(dur);
            }
        }

        public void ExportPhoneSeq(List<string> phoneSeq)
        {
            for (int i = 0; i < _Phones.Count; ++i)
            {
                phoneSeq.Add(_Phones[i]);
            }
        }

        public void Exportphrase2syllables(List<List<TtsSyllable>> phrase2Syllables)
        {
            for (int i = 0; i < _phrase2Syllables.Count; ++i)
            {
                phrase2Syllables.Add(_phrase2Syllables[i]);
            }
        }


        public void Exportphrase2posStress(List<int> phrase2PosStress)
        {
            for (int i = 0; i < wordPOS2Stress.Count; ++i)
            {
                for (int j = 0; j < wordPOS2Stress[i].Count; j++)
                    phrase2PosStress.Add(wordPOS2Stress[i][j]);
            }
        }
        public void ExportSyllableSeqttsPhone(List<List<bool>> syllable2IsVowelPhones)
        {
            for (int i = 0; i < _Syllable2IsVowelPhones.Count; ++i)
            {
                List<bool> isVowelphones = new List<bool>();
                foreach (var item in _Syllable2IsVowelPhones[i])
                {
                    bool isVowel = item;
                    isVowelphones.Add(isVowel);
                }
                syllable2IsVowelPhones.Add(isVowelphones);
            }
        }

        public void ExportSyllableSeq(List<List<string>> syllable2phones)
        {
            for (int i = 0; i < _Syllable2Phones.Count; ++i)
            {
                syllable2phones.Add(_Syllable2Phones[i]);
            }
        }
        public void ExportSyllableVNSeq(List<List<string>> syllable2phonesnew)
        {
            for (int i = 0; i < syllable2phones.Count; ++i)
            {
                syllable2phonesnew.Add(syllable2phones[i]);
            }
        }
        //get phrase
        public void ExportPhraseGroups(List<List<TtsSyllable>> phrase2Syllables,List<List<int>>phraseGroups)
        {
            int progress = 0;
            for (int i = 0; i < phrase2Syllables.Count; ++i)
            {
                List<int>phraseindex = new List<int>();
                if(i==0)
                {
                    for (int index = 0; index < phrase2Syllables[0].Count(); index++)
                    {
                        phraseindex.Add(index+1);
                    }
                }
                else
                {
                    for(int jdex=0;jdex< phrase2Syllables[i].Count();jdex++)
                    {
                        phraseindex.Add(progress + jdex+1);
                    }
                }
               
                phraseGroups.Add(phraseindex);
                progress = progress + phrase2Syllables[i].Count();
            }
        }
        public void ExportSyllableStress(List<int> syllable2stress)
        {
            for (int i = 0; i < _stress.Count; ++i)
            {
                syllable2stress.Add(_stress[i]);
            }
        }

        public void Exportworktext(List<string> wordtext)
        {
            for (int i = 0; i < WordString.Count; ++i)
            {
                wordtext.Add(WordString[i]);
            }
        }
        public void ExportSyllableAccent(List<int> syllable2Accent)
        {
            for (int i = 0; i < _accent.Count; ++i)
            {
                syllable2Accent.Add(_accent[i]);
            }
        }
        public void ExportSyllablePhoneArrary(List<string> SyllablePhoneArrary)
        {
            for (int i = 0; i < _Syllable2phoneArrary.Count; ++i)
            {
                SyllablePhoneArrary.Add(_Syllable2phoneArrary[i]);
            }
        }

        public void ExportSyllablePhoneArraryVN(List<string> SyllablePhoneArrary)
        {
            for (int i = 0; i < _Syllable2phoneArraryVN.Count; ++i)
            {
                SyllablePhoneArrary.Add(_Syllable2phoneArraryVN[i]);
            }
        }

        //phrase rule 20170703,get the end syllableSeq
        //public void Align_stress_syllable_rule(List<int> syllableStressIndex, List<int> syllableSeqindex, List<List<int>> phraseGroups)
        //{
        //    for(int i=0;i<syllableStressIndex.Count();i++)
        //    {
        //      if (i==0)
        //        {

        //            if(syllableStressIndex[i] != 0&&syllableStressIndex[i]%2==0)
        //            {
        //                syllableSeqindex.Insert(0,0);
        //                /*_phoneSequence.Insert(0, "sil"); */  //add silence at the begin
        //                //phoneDurIndexlist.Insert(0,);
        //            }

        //        }
        //      else
        //        {
        //            int diffbefore = syllableStressIndex[i] - syllableStressIndex[i - 1] - 1; //syllable numbers
        //            if (diffbefore == 0)
        //            {
        //                int value = syllableStressIndex[i];
        //                int index = syllableSeqindex.IndexOf(value);
        //                syllableSeqindex.Insert(index, 0);
        //            }
        //            else if (diffbefore == 1) continue;
        //            else
        //            {
        //                //int NonstressAndBeats = (diffbefore - 1) / 2 + (diffbefore - 1) % 2;
        //                if((diffbefore - 1) % 2!=0)
        //                judge_non_stress_group(syllableStressIndex[i - 1], syllableStressIndex[i], phraseGroups, syllableSeqindex);

        //            }
        //        }


        //    }

        //}

        //public void judge_non_stress_group(int start,int end, List<List<int>> phraseGroups, List<int> syllableSeqindex)
        //{
        //    int start_inGroup = 0;
        //    int end_inGroup = 0;
        //    int iflag_find = 0;
        //    for(int m=0;m< phraseGroups.Count();m++)
        //    {
        //        if (phraseGroups[m].Contains(start))
        //            start_inGroup = m;
        //        if (phraseGroups[m].Contains(end))
        //            end_inGroup = m;
        //    }
        //    if (start_inGroup == end_inGroup)
        //    {
        //        int indexay = syllableSeqindex.IndexOf(end);
        //        syllableSeqindex.Insert(indexay, 0);
        //    }    
        //    else
        //    {
        //        for(int data= start+1;data< end;data++)
        //        {
        //            if(phraseGroups[end_inGroup].Contains(data))
        //            {
        //                int index = syllableSeqindex.IndexOf(data);
        //                syllableSeqindex.Insert(index, 0);
        //                iflag_find = 1;
        //                break;
        //            }

        //        }
        //        if(iflag_find==0)
        //            syllableSeqindex.Insert(end, 0);

        //    }



        //}
        ////spile list to stress and non stress list
        //public void splite_stressAndNonstressList(List<int> stressList, List<int> syllableStressIndex, List<int> syllableSeqIndex)
        //{
        //    for(int i=0;i< stressList.Count();i++)
        //    {
        //        if (stressList[i] == 1)
        //            syllableStressIndex.Add(i+1);
        //        syllableSeqIndex.Add(i+1);
        //    }
        //}
        ////add sil
        //public void nominizetion_phoneseq(List<int> syllableSeqindex,List<List<string>> syllable2phones, List<List<int>> phonedurs2line_List)
        //{
        //    List<string> addsil = new List<string>();
        //    addsil.Add("sil");
        //    List<int> intsil = new List<int>();
        //    intsil.Add(40);
        //    for(int i=0;i< syllableSeqindex.Count();i++)
        //    {
        //        if (syllableSeqindex[i] == 0)
        //        {

        //                syllable2phones.Insert(i, addsil);
        //                phonedurs2line_List.Insert(i, intsil);

        //        }


        //    }
        //}
        private void OnProcessed(object sender, TtsModuleEventArgs e)
        {
            Console.WriteLine(e.ModuleType);
            if (e.ModuleType == TtsModuleType.TM_WAVE_GENERATION)
            {
                //// the utt instance must be disposed in the end, or else it will lead engine crash.
                using (var utt = CacheUtterance(e.Utterance))
                {
                    var engine = (TtsEngine)sender;

                    foreach (TtsPhone phone in utt.Phones)
                    {
                        
                        if (phone.Name == "-SIL-" || phone.Name == "sil" || phone.Name == "-sil-")
                            _Phones.Add("sil");
                        else
                            _Phones.Add(phone.Name.ToLower());
                    }

                    if(wordPOS2Stress.Count!=0&& (syllable2phones[syllable2phones.Count - 1][0] == "sil") && (syllable2phones[syllable2phones.Count - 1].Count == 1))
                    {
                        _phrase2Syllables.RemoveAt(_phrase2Syllables.Count - 1);
                        wordPOS2Stress.RemoveAt(wordPOS2Stress.Count - 1);
                        WordString.RemoveAt(WordString.Count - 1);
                        syllable2phones.RemoveAt(syllable2phones.Count - 1);
                    }

                    //phrase to syllables
                    foreach (TtsPhrase phrase in utt.Phrases)
                    {
                        //phrase 2 words
                        List<List<int>>posStressList=new List<List<int>>();
                        List<int> wordPOS = new List<int>();
                        List<TtsWord> wordlist = new List<TtsWord>();
                        List<TtsSyllable> syllableList = new List<TtsSyllable>();
                        List<string> w2phones = new List<string>();
                        TtsWord _word = phrase.FirstWord;
                        while(_word!=null)
                        {
                            wordlist.Add(_word);
                            if (_word == phrase.LastWord)
                                break;
                            _word = _word.Next;
                        }
                        //words to syllables
                        foreach(TtsWord world in wordlist)
                        {
                            TtsSyllable _syllable = world.FirstSyllable;
                            while(_syllable!=null)
                            {
                                int istress = 0;
                                //convert word POS to stress
                                if(world.Pos<=8&&world.Pos>=1)
                                {
                                    switch (_syllable.Stress)
                                    {
                                        case SP.TtsStress.STRESS_NONE:
                                            istress = 0;
                                            break;
                                        default:
                                            istress = 1;
                                            break;
                                    }
                                }
                                else
                                {
                                    istress = 0;
                                }
                                //0720
                                wordPOS.Add(istress);
                                if (world.WordText == "" || world.WordText == " ")
                                    WordString.Add("sil");
                                else
                                    WordString.Add(world.WordText);

                                syllableList.Add(_syllable);
                                if (_syllable == world.LastSyllable) break;
                                _syllable = _syllable.Next;
                            }
                        }
                        _phrase2Syllables.Add(syllableList);
                        wordPOS2Stress.Add(wordPOS);

                        //syllables 2 phones
                        foreach (TtsSyllable syllable in syllableList)
                        {
                            List<string> phones = new List<string>();
                            TtsPhone phonew = syllable.FirstPhone;
                            while (phonew != null)
                            {
                                if (phonew.Name == "-SIL-" || phonew.Name == "sil" || phonew.Name == "-sil-")
                                {
                                    w2phones.Add("sil");
                                    phones.Add("sil");
                                }
                                   
                                else
                                {
                                    phones.Add(phonew.Name.ToLower());
                                    w2phones.Add(phonew.Name.ToLower());
                                }
                                    
                                if (phonew == syllable.LastPhone)
                                {
                                    break;
                                }
                                phonew = phonew.Next;
                            }

                            //hongmei 20170630
                            string strsyllable2phones = "";
                            foreach (string strphone in phones)
                            {
                                if (strsyllable2phones == "")
                                    strsyllable2phones = strphone;
                                else
                                    strsyllable2phones = strsyllable2phones + "_" + strphone;

                            }
                            _Syllable2phoneArraryVN.Add(strsyllable2phones);
                        }
                        syllable2phones.Add(w2phones);  

                    }
                    //delete sil
                    //if ((syllable2phones[0][0] == "sil") && (syllable2phones[0].Count == 1))
                    //{

                    //    _phrase2Syllables.RemoveAt(0);
                    //    wordPOS2Stress.RemoveAt(0);
                    //    WordString.RemoveAt(0);
                    //    syllable2phones.RemoveAt(0);



                    //}
                    //if ((syllable2phones[syllable2phones.Count - 1][0] == "sil") && (syllable2phones[syllable2phones.Count - 1].Count == 1))
                    //{
                    //    _phrase2Syllables.RemoveAt(_phrase2Syllables.Count - 1);
                    //    wordPOS2Stress.RemoveAt(wordPOS2Stress.Count - 1);
                    //    WordString.RemoveAt(WordString.Count - 1);
                    //    syllable2phones.RemoveAt(syllable2phones.Count - 1);

                    //}
                    //int icout = 0;
                    //for(int isil=0;isil<syllable2phones.Count;isil++)
                    //{
                    //    if((syllable2phones[isil][0]=="sil")&& (syllable2phones[isil].Count == 1))
                    //        {
                    //        _phrase2Syllables.RemoveAt(isil-icout);
                    //        //syllable2phones.RemoveAt(isil - icout);
                    //        icout++;
                    //        }

                    //}
                    if(_Syllable2Phones.Count!=0)
                    {
                        _Syllable2Phones.RemoveAt(_Syllable2Phones.Count-1);
                        _Syllable2IsVowelPhones.RemoveAt(_Syllable2IsVowelPhones.Count-1);
                        _stress.RemoveAt(_stress.Count-1);
                        _accent.RemoveAt(_accent.Count-1);
                        _Syllable2phoneArrary.RemoveAt(_Syllable2phoneArrary.Count-1);
                    }
                     foreach (TtsSyllable syllable in utt.Syllables)
                    {
                        List<string> phones = new List<string>();
                        List<bool> isVowel = new List<bool>();
                        int isstress = 0;
                        int isAccont = 0;
                        TtsPhone phone = syllable.FirstPhone;
                        while (phone != null)
                        {
                            isVowel.Add(phone.IsVowel);
                            
                            if (phone.Name == "-SIL-" || phone.Name == "sil" || phone.Name == "-sil-")
                                phones.Add("sil");
                            else
                                phones.Add(phone.Name.ToLower());
                            if (phone == syllable.LastPhone)
                            {
                                break;
                            }
                            phone = phone.Next;
                        }

                        //hongmei 20170630
                        string strsyllable2phones = "";
                        foreach(string strphone in phones)
                        {
                            if (strsyllable2phones == "")
                                strsyllable2phones = strphone;
                            else
                                strsyllable2phones = strsyllable2phones + "_" + strphone;
                         
                        }
                        ////syllable to find preposition word 
                        //int iflag = 0;
                        //foreach(string prep in prepArrary)
                        //{
                        //    if (syllable.Word.WordText.ToLower() == prep)
                        //    {
                        //        isstress = 0;
                        //        isAccont = 0;
                        //        iflag = 1;
                        //    }     
                        //}
                        
                        //if(iflag==0)
                        //{
                            switch (syllable.Stress)
                            {
                                case SP.TtsStress.STRESS_NONE:
                                    isstress = 0;
                                    break;
                                case SP.TtsStress.STRESS_PRIMARY:
                                    isstress = 1;
                                    Console.WriteLine("syllable {0}:stress style is STRESS_PRIMARY", strsyllable2phones);
                                    break;
                                case SP.TtsStress.STRESS_SECONDARY:
                                    isstress = 1;
                                    Console.WriteLine("syllable {0}:stress style is STRESS_SECONDARY", strsyllable2phones);
                                    break;
                                case SP.TtsStress.STRESS_TERTIARY:
                                    isstress = 1;
                                    Console.WriteLine("syllable {0}:stress style is STRESS_TERTIARY", strsyllable2phones);
                                    break;
                            }
                            switch (syllable.ToBIAccent)
                            {
                                case SP.TtsTobiAccent.K_NOACC:
                                    isAccont = 0;
                                    break;
                                default:
                                    isAccont = 1;
                                    break;

                            }
                        //}
                        
                        _Syllable2Phones.Add(phones);
                        _Syllable2IsVowelPhones.Add(isVowel);
                        _stress.Add(isstress);
                        _accent.Add(isAccont);
                        _Syllable2phoneArrary.Add(strsyllable2phones);
                    }

                    // copy features
                    if (_durations.Count != 0)
                        _durations.RemoveAt(_durations.Count - 1);
                    for (int i = 0; i < utt.Acoustic.Durations.Row; ++i)
                    {
                        uint[] durations = new uint[utt.Acoustic.Durations[i].Length];
                        for (int j = 0; j < durations.Length; ++j)
                        {
                            durations[j] = utt.Acoustic.Durations[i][j];
                        }
                        _durations.Add(durations);
                    }
                }
            }
        }

        private SP.TtsUtterance CacheUtterance(SP.TtsUtterance utterance)
        {
            SP.TtsUtterance utt = new SP.TtsUtterance(utterance);
            utt.CacheOriginalText();
            return utt;
        }

        #endregion
    }
}
