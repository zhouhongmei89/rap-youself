using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Tts.Offline;
using Microsoft.Tts.Offline.Utility;
using SP = Microsoft.Tts.ServiceProvider;
using Microsoft.Tts.ServiceProvider;
using _Jie_OfflineTools;
using System.IO;

namespace RapYouself
{
    class Program
    {
        public delegate void Write_Beat_log(string szlog);
        public static event Write_Beat_log WriteBeatEvent;

        private static featureExport _featureExport;
        public const int halfbeat = 40;

        //public  int imeasure = 0;
        //public  int ibeat = 0;
        private static List<double> daddsil = new List<double>();
        static void Main(string[] args)
        {
            //if (args.Length >= 1)
            //{
                //string file = args[0];
                //if (File.Exists(file))
                //{
                    //add path to synsis
                    string textDir = @"E:\Work\rapyouself2\RapYouself_vn_815\NUS\text";
                    string f0Dir = @"E:\Work\rapyouself2\RapYouself_vn_815\NUS\f0";
                    string offlineDir = @"E:\offline_0629";
                    string script2wavConf = @"E:\Work\rapyouself2\RapYouself_vn_815\NUS\ScriptToWave.config";
                    string outputDir = @"E:\Work\rapyouself2\RapYouself_vn_815\NUS\Samples";
                    string FfmpegPath = @"E:\TTS_tool\ffmpeg-20161218-02aa070-win64-static\bin\ffmpeg.exe";

                    string outputfile = "Syllables_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".wav";
                   
                     string WorkDir = @"E:\Work\rapyouself2\RapYouself_vn_815";
                    string fontPath = @"E:\Work\rapyouself2\RapYouself_vn_815\zo\1033";
                    string localHandlerPath = @"E:\Work\rapyouself2\RapYouself_vn_815\zo";
                    string outAlignment = Path.Combine(WorkDir, @"outAlignment");
                    string subWavFileList = Path.Combine(WorkDir, @"TemplateWavs");
                    string logpath = Path.Combine(WorkDir, @"log");
               // string wavtext = Path.Combine(textDir, @"wavtext");


                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    if(!Directory.Exists(outAlignment))
                    {
                        Directory.CreateDirectory(outAlignment);
                    }
                    //if(!Directory.Exists(wavtext))
                    //  {
                    //Directory.CreateDirectory(wavtext);
                    //   }
                    //else
                    //{
                    //    Directory.Delete(outputDir, true);
                    //    Directory.CreateDirectory(outputDir);
                    //}

                if (!Directory.Exists(logpath))
                {
                    Directory.CreateDirectory(logpath);
                }
                
                string outlogfile = "log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                string logpathfile = Path.Combine(logpath, outlogfile);

                if (!Directory.Exists(subWavFileList))
                    {
                        Directory.CreateDirectory(subWavFileList);
                    }
                    
                    List<string> sentences = new List<string>();
                    using (StreamReader sr = new StreamReader(Path.Combine(textDir,"0000000001.txt")))
                    {
                    string sentence = "";
                    do
                    {
                        sentence = sr.ReadLine();
                        if (sentence == "" || sentence == null)
                            break;
                        sentence = sentence.Trim();
                        sentences.Add(sentence);
                    } while (sentence != "" || sentence != null);
                    //string lylic = sr.ReadToEnd();
                    //lylic = lylic.Trim();
                    //lylic = lylic.Replace("\r\n", "");
                    //char[] spli = {'.', '?' };
                    //string[] tempsentences = lylic.Split(spli);
                }
                string filelist = subWavFileList + "\\filelist.txt";
                if (File.Exists(filelist))
                    File.Delete(filelist);
                Microsoft.Tts.Offline.Language lang = Localor.StringToLanguage("en-us");
                string mapfile = Path.Combine(WorkDir, @"map.txt");
                if (File.Exists(mapfile))
                    File.Delete(mapfile);
            string beatfilename = "measure_beat_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string beatpath = Path.Combine(@"E:\Work\workdir\Beat", beatfilename);

            WriteBeatEvent += delegate (string szlog)
              {
                  try
                  {
                      using (StreamWriter sw = new StreamWriter(beatpath, true, Encoding.UTF8))
                      {
                          sw.WriteLine(szlog);
                      }
                  }
                  catch(IOException e)
                  {
                      Console.WriteLine(e.Message);
                  }
              };
            //_featureExport = new featureExport(serviceProvider, lang);
            //_featureExport.PrepareSpeak();
            for (int i=0;i< sentences.Count;i++)
                    {

                    using (StreamWriter swmap = new StreamWriter(mapfile, true))
                    {
                        string line = (i + 1).ToString("D10") + " " + (i + 1).ToString("D10");
                        swmap.WriteLine(line);
                    }


                        evaSinging_generateAlignment_beatRule(sentences[i], logpathfile, outAlignment, lang, fontPath, localHandlerPath, (i + 1).ToString("D10") + ".txt", beatfilename);

                        using (StreamWriter sw = new StreamWriter(filelist, true))
                        {
                            string filepath = Path.Combine(WorkDir, @"Output_script2wav", (i + 1).ToString("D10") + @".wav");
                            if (File.Exists(filepath))
                                File.Delete(filepath);
                            sw.WriteLine(string.Format("file '{0}'", filepath));

                        }

                        //text = text.Replace(" ", "");
                        //string outttsWave = Path.Combine(outputDir, @"out_" + text + @".wav");

                        //arg = string.Format("-f concat -safe 0 -i {0} -c copy {1}", subWavFileList, outttsWave);
                        //CommandLine.RunCommand(FfmpegPath, arg, null);
                  
                    
                }
                evaSinging_ProcessScript("en-us", textDir, WorkDir, offlineDir, outAlignment, f0Dir, script2wavConf);
                string outttsWave = Path.Combine(outputDir, outputfile);
                string arg = string.Format("-f concat -safe 0 -i {0} -c copy {1}", filelist, outttsWave);
                CommandLine.RunCommand(FfmpegPath, arg, null);


            //}
            //    else
            //    {
            //        Console.WriteLine("Error,{0} file is not exit");
            //    }
            //}
        }
        private static void evaSinging_generateAlignment_beatRule(string text, string logpathfile, string outAlignmentDir, Microsoft.Tts.Offline.Language lang, string fontPath, string LocaleHandlerDir, string outputFileName,string beatfile)
        {
            try
            {
                List<List<string>> syllable2phones = new List<List<string>>();
                List<List<string>> syllable2VNphones = new List<List<string>>();
                //0803hongmei
                List<string> wordtext = new List<string>();
                List<List<bool>> syllable2IsVowelPhones = new List<List<bool>>();
                List<List<int>> syllable2phoneDur = new List<List<int>>();
                List<List<int>> phonedurs2line_List = new List<List<int>>();
                List<int> Sysllable2stressIndex = new List<int>();
                List<int> _tts_phonedurations = new List<int>();

                List<int> _linear_phonedurations_stress = new List<int>();
                List<string> _phoneSeq = new List<string>();
                List<string> _wordstring = new List<string>();
                //hongmei 
                List<string> syllablePhoneSeq = new List<string>();
                List<string> syllablePhoneVNSeq = new List<string>();
                List<int> SyllablePosStress = new List<int>();
                List<int> Syllable2stress = new List<int>();
                List<int> Syllable2Accent = new List<int>();
                List<List<TtsSyllable>> Phrase2syllables = new List<List<TtsSyllable>>();
                List<List<int>> phraseGroups = new List<List<int>>();
                List<List<int>> stressIndexpos = new List<List<int>>();
                List<List<int>> stressIndex = new List<List<int>>();
                List<List<int>> accentIndex = new List<List<int>>();
                List<int> syllableAllIndex = new List<int>();
                List<int> syllableAllIndex1 = new List<int>();
                List<int> syllableAllIndex2 = new List<int>();
                List<int> liner_dur = new List<int>();
                using (ServiceProvider serviceProvider = new ServiceProvider((SP.Language)lang, fontPath, LocaleHandlerDir))
                {
                    _featureExport = new featureExport(serviceProvider, lang);
                    _featureExport.PrepareSpeak();

                    serviceProvider.SpeechSynthesizer.Speak(text);
                    _featureExport.ExportDuration(_tts_phonedurations);
                    _featureExport.ExportSyllableSeq(syllable2phones);
                    _featureExport.Exportworktext(wordtext);
                
                    _featureExport.ExportSyllableVNSeq(syllable2VNphones);
                    //_featureExport.ExportSyllableSeqttsPhone(syllable2IsVowelPhones);
                    _featureExport.ExportSyllableStress(Syllable2stress);
                    _featureExport.Exportphrase2posStress(SyllablePosStress);
                    _featureExport.ExportSyllableAccent(Syllable2Accent);
                    _featureExport.Exportphrase2syllables(Phrase2syllables);
                    
                    //_featureExport.splite_stressAndNonstressList(Syllable2stress, stressIndex, syllableAllIndex);
                    //_featureExport.Align_stress_syllable_rule(stressIndex, syllableAllIndex, phraseGroups);


                }

                    if ((syllable2phones[0][0] == "sil") && (syllable2phones[0].Count == 1))
                    {
                        syllable2phones.RemoveAt(0);
                        //syllable2IsVowelPhones.RemoveAt(0);
                        _tts_phonedurations.RemoveAt(0);
                        Syllable2stress.RemoveAt(0);
                        //SyllablePosStress.RemoveAt(0);
                        Syllable2Accent.RemoveAt(0);
                    SyllablePosStress.RemoveAt(0);
                    Phrase2syllables.RemoveAt(0);
                    wordtext.RemoveAt(0);
                    }
                if ((syllable2phones[syllable2phones.Count - 1][0] == "sil") && (syllable2phones[syllable2phones.Count - 1].Count == 1))
                {
                    syllable2phones.RemoveAt(syllable2phones.Count - 1);
                    //syllable2IsVowelPhones.RemoveAt(syllable2IsVowelPhones.Count - 1);
                    _tts_phonedurations.RemoveAt(_tts_phonedurations.Count - 1);
                    Syllable2stress.RemoveAt(Syllable2stress.Count - 1);
                    Syllable2Accent.RemoveAt(Syllable2Accent.Count - 1);
                    SyllablePosStress.RemoveAt(SyllablePosStress.Count - 1);
                    Phrase2syllables.RemoveAt(Phrase2syllables.Count - 1);
                    wordtext.RemoveAt(wordtext.Count - 1);
                    //SyllablePosStress.RemoveAt(SyllablePosStress.Count - 1);
                }

                int phoneId = 0;
                foreach (var item in syllable2phones)
                {
                    string syllablestring = "";
                    List<int> phoneDur = new List<int>();
                    foreach (var subItem in item)
                    {
                        phoneDur.Add(_tts_phonedurations[phoneId++]);
                        if (syllablestring == "")
                            syllablestring = subItem;
                        else
                            syllablestring = syllablestring + "_" + subItem;
                        //_phoneSeq.Add(subItem);
                    }
                    syllable2phoneDur.Add(phoneDur);
                    syllablePhoneSeq.Add(syllablestring);
                }


    


                if (phoneId != _tts_phonedurations.Count)
                {
                    Console.WriteLine("Error");
                }

                _featureExport.ExportPhraseGroups(Phrase2syllables, phraseGroups);
                //string phonename = Path.GetFileNameWithoutExtension(outputFileName);
                splite_stressAndNonstressList(SyllablePosStress, stressIndexpos, syllableAllIndex, phraseGroups);
                splite_stressAndNonstressList(Syllable2stress, stressIndex, syllableAllIndex1, phraseGroups);

                find_stress_beatween_continous_5nonstress(stressIndexpos, stressIndex, phraseGroups);
                //splite_stressAndNonstressList(Syllable2Accent, accentIndex, syllableAllIndex2, phraseGroups);

                //Align_stress_syllable_rule(stressIndexpos, syllableAllIndex, phraseGroups, syllable2phoneDur, phonedurs2line_List, stressIndex, syllablePhoneSeq, beatfile,wordtext);
                Align_stress_syllable_rule_new(stressIndexpos, syllableAllIndex, phraseGroups, syllable2phoneDur, phonedurs2line_List, stressIndex, syllablePhoneSeq, wordtext);
                nominizetion_phoneseq(syllableAllIndex, syllable2phones, phonedurs2line_List);

                using (StreamWriter sw = new StreamWriter(logpathfile, true))
                {
                    for (int i = 0; i < phraseGroups.Count; i++)
                    {
                        foreach (int dex in phraseGroups[i])
                        {
                            sw.WriteLine("G{0}:{1}", i, dex);
                        }
                        sw.WriteLine();
                        foreach(int pos in stressIndexpos[i])
                        {
                            sw.WriteLine("PosStress:{0}", pos);
                        }
                        sw.WriteLine();
                        foreach (int strs in stressIndex[i])
                        {
                            sw.WriteLine("stress:{0}", strs);
                        }


                    }
                    
                    sw.WriteLine();
                   

                    foreach (List<int> item in phonedurs2line_List)
                    {
                        string durstring = "";
                        foreach (var dur in item)
                        {
                            if (durstring == "")
                                durstring = dur + "";
                            else
                                durstring = durstring + "_" + dur;
                            liner_dur.Add(dur);
                        }
                        sw.WriteLine(durstring);

                    }

                    int countword = 0;
                    foreach (List<string> item_name in syllable2phones)
                    {
                        int iflag = 0;
                        string phones = "";
                        foreach (string phone in item_name)
                        {
                            if (phones == "")
                                phones = phone;
                            else
                                phones = phones + "_" + phone;
                            _phoneSeq.Add(phone);
                            if (phones == "sil" && wordtext[countword] != "sil")
                            {
                                iflag = 1;
                                _wordstring.Add("sil");
                                continue;
                            }
                            else
                                _wordstring.Add(wordtext[countword]);

                        }
                        sw.WriteLine(phones);
                        if(iflag==0)
                            countword++;                  
                    }
                }

                using (StreamWriter file = new StreamWriter(Path.Combine(outAlignmentDir, outputFileName), false))
                {
                    double starttime = 0;
                    for (int i = 0; i < liner_dur.Count; ++i)
                    {
                        double stime = 0.0;
                        //add beat before sentence
                        //if (i == 0)
                        //{
                        //    if (_phoneSeq[0] == "sil")
                        //    {
                        //        stime = (starttime * 5 / 1000);
                        //        starttime = 2 * halfbeat;
                        //    }
                        //    else
                        //    {
                        //        stime = (starttime * 5 / 1000);
                        //        file.WriteLine(Math.Round(stime, 5).ToString("0.00000") + " " + "sil");
                        //        starttime = 2 * halfbeat;
                        //        stime = (starttime * 5 / 1000);
                        //    }

                        //}
                        //else
                        //{
                            stime = (starttime * 5 / 1000);
                        //}

                        file.WriteLine(Math.Round(stime, 5).ToString("0.00000") + " " + _phoneSeq[i]);
                        starttime += liner_dur[i];
                    }
                    file.WriteLine(Math.Round((starttime * 5 / 1000), 5).ToString("0.00000") + " " + "sil");
                }


                
                using (StreamWriter file = new StreamWriter(Path.Combine(@"E:\Work\output", @"word2phones.txt"), false))
                {
                    double starttime = 0;
                    for (int i = 0; i < liner_dur.Count; ++i)
                    {
                        double stime = 0.0;
                       
                        stime = (starttime * 5 / 1000);

                            file.WriteLine(Math.Round(stime, 5).ToString("0.00000") + " " + _phoneSeq[i]+"\t "+_wordstring[i]);
                        
                        starttime += liner_dur[i];
                    }
                    file.WriteLine(Math.Round((starttime * 5 / 1000), 5).ToString("0.00000") + " " + "sil");
                }


            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


}

        private static void evaSinging_LinearScale(List<int> sDuration, List<int> dDuration, int targetDuration)
        {
            int _tts_totalDur = 0;
            foreach (var item in sDuration)
            {
                _tts_totalDur += item;
            }

            double ratio = targetDuration / (Double)_tts_totalDur;
            int curTotalDur = 0;
            foreach (var item in sDuration)
            {
                int linear_phoneduration = (int)(item * ratio);
                if (linear_phoneduration == 0)
                {
                    linear_phoneduration = 1;
                }
                dDuration.Add(linear_phoneduration);
                curTotalDur += linear_phoneduration;
            }

            int diff = targetDuration - curTotalDur;

            int idx = 0;
            while (diff != 0)
            {
                int id = idx % sDuration.Count;
                if (diff > 0)
                {
                    dDuration[id]++;
                    diff--;
                }
                else
                {
                    if (dDuration[id] > 1)
                    {
                        dDuration[id]--;
                        diff++;
                    }
                }
                idx++;
            }
        }

        private static bool evaSinging_ProcessScript(string Language, string textDir, string workDir, string offlineDir, string alignmentDir, string f0Dir, string script2WavConf)
        {
            try
            {
                string arg;

                string XmlPath = Path.Combine(workDir, @"XmlPath");
                string PronPath = Path.Combine(workDir, @"PronPath");
                string DotPath = Path.Combine(workDir, @"DotPath");
                string PhonePath = Path.Combine(workDir, @"PhonePath");

                string ScriptProcessor = Path.Combine(offlineDir, @"ScriptProcessor.exe");
                string ScriptTagger = Path.Combine(offlineDir, @"ScriptTagger.exe");
                string SliceGenerator = Path.Combine(offlineDir, @"SliceGenerator");
                string ScriptAcousticTagger = Path.Combine(offlineDir, @"ScriptAcousticTagger");

                string ScriptPronReport = Path.Combine(PronPath, @"report.txt");
                string ScriptPhoneReport = Path.Combine(PhonePath, @"report.txt");

                // To xml
                arg = string.Format("-mode converttoxml -lang {0} -inscriptdir \"{1}\" -outscriptdir \"{2}\" -inscriptwithoutpron -inscriptwithoutsid",
                    Language, textDir, XmlPath);
                CommandLine.RunCommand(ScriptProcessor, arg, null);
                // Add pronunciation
                arg = string.Format("-mode pronunciation -lang {0} -sd \"{1}\" -td \"{2}\" -report \"{3}\"",
                    Language, XmlPath, PronPath, ScriptPronReport);
                CommandLine.RunCommand(ScriptTagger, arg, null);
                // Add dot
                arg = string.Format("-l {0} -phonebased -sd \"{1}\" -td \"{2}\"",
                    Language, PronPath, DotPath);
                CommandLine.RunCommand(SliceGenerator, arg, null);
                // Extend to phone level
                arg = string.Format("-mode extendorremovescriptlayer -modifyscriptlayermode Extend -scriptlayer phone -inscriptdir \"{0}\" -outscriptdir \"{1}\" -report \"{2}\"",
                    DotPath, PhonePath, ScriptPhoneReport);
                CommandLine.RunCommand(ScriptProcessor, arg, null);

                if (!File.Exists(Path.Combine(PhonePath, "0000000001.xml")))
                {
                    return false;
                }

                // acoustic tagger

                string durationTaggerPath = Path.Combine(workDir, @"durationTaggerPath");
                string f0TaggerPath = Path.Combine(workDir, @"f0TaggerPath");
                string mapperFile = Path.Combine(workDir, @"map.txt");

                string importsegmentreport = Path.Combine(durationTaggerPath, "importsegment-report.txt");
                arg = string.Format("-mode ImportSegment -lang {0} -inscriptdir \"{1}\" -layerToTag phone -importDataDir \"{2}\"" +
                            " -looseVowelCompare false -loosephonecompare false -addDuration true -addSilence true -mapFile \"{3}\" -outScriptDir \"{4}\" -reportFile \"{5}\"",
                            Language, PhonePath, alignmentDir, mapperFile, durationTaggerPath, importsegmentreport);
                CommandLine.RunCommand(ScriptAcousticTagger, arg, null);

                //string importf0report = Path.Combine(f0TaggerPath, "importf0-report.txt");
                //arg = string.Format("-mode ImportF0 -lang {0} -inscriptdir \"{1}\" -layerToTag phone -f0DataSet External -sampleRate 16000 -frameSize 0.005" +
                //            " -importDataDir \"{2}\" -mapFile \"{3}\" -f0EncodingMode Text -outScriptDir \"{4}\" -reportFile \"{5}\"",
                //            Language, durationTaggerPath, f0Dir, mapperFile, f0TaggerPath, importf0report);
                ////CommandLine.RunCommand(ScriptAcousticTagger, arg, null);


                //script2wav

                arg = string.Format("-config {0}", script2WavConf);
                string ScriptToWave = Path.Combine(offlineDir, @"ScriptToWave");

                CommandLine.RunCommand(ScriptToWave, arg, null);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
           
        }
        public static void strench_1beat1syllable(int start, List<List<int>> primaryDur, List<List<int>> lineScarDur,ref int imeasure, ref double ibeat,int Gindex, List<string> wordtext,ref int iflag)
        {
           
                if (Gindex != 0|| iflag==1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[start - 1]));
                ibeat = ibeat + 1;

                }
                else
                {
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m0 bt4:{0}", wordtext[start - 1]));
                    iflag = 1;
                ibeat = 5;
            }
           

            

            List<int> _liner_dur0 = new List<int>();
            evaSinging_LinearScale(primaryDur[start - 1], _liner_dur0, halfbeat * 2);
            lineScarDur.Add(_liner_dur0);

        }

        public static void strench_1beat2syllable(int start, int end,List<List<int>> primaryDur, List<List<int>> lineScarDur, ref int imeasure, ref double ibeat, int Gindex, List<string> wordtext,ref int iflag)
        {
            int count = 1;
            for (int ii = start; ii < end; ii++)
            {
                //write beat log
                if (Gindex != 0||iflag==1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]));

                    ibeat = ibeat + 1;
                }
                else
                {
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m0 bt{0}:{1}", 5 - (3 - ii) * 0.5, wordtext[ii - 1]));
                    ibeat = 5 - (3 - count) ;
                    iflag = 1;
                }

                List<int> _liner_dur2 = new List<int>();
                evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat*2);
                lineScarDur.Add(_liner_dur2);

                count++;

            }
        }

        public static void strench_1beat2syllable_0(int start, int end, List<List<int>> primaryDur, List<List<int>> lineScarDur, ref int imeasure, ref double ibeat, int Gindex, List<string> wordtext, ref int iflag)
        {
            int count = 1;
            for (int ii = start; ii < end; ii++)
            {
                //write beat log
                if (Gindex != 0 || iflag == 1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]));

                    ibeat = ibeat + 0.5;
                }
                else
                {
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m0 bt{0}:{1}", 5 - (3 - count) * 0.5, wordtext[ii - 1]));
                    ibeat = 5 - (3 - count)*0.5+0.5;
                    iflag = 1;
                }

                List<int> _liner_dur2 = new List<int>();
                evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                lineScarDur.Add(_liner_dur2);

                count++;

            }
        }

        public static void strench_2beat3syllable(int start, int end, List<List<int>> primaryDur, List<List<int>> lineScarDur, ref int imeasure, ref double ibeat, int Gindex,List<string> wordtext,ref int iflag)
        {
            int icount = 1;
            int count = 1;
            for (int ii = start; ii <= start + 1; ii++)
            {
                //write beat log
                if (Gindex != 0||iflag==1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]));

                    ibeat = ibeat + 0.5;
                }
                else
                {
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m0 bt{0}:{1}", 4 - (3 - icount) * 0.5, wordtext[ii - 1]));
                    ibeat = 4 - (3 - icount) * 0.5+0.5;
                }
                List<int> _liner_dur11 = new List<int>();
                evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                lineScarDur.Add(_liner_dur11);
                icount++;
            }
            //write beat log
            if (Gindex != 0||iflag==1)
            {
                if (ibeat == 5)
                {
                    imeasure = imeasure + 1;
                    ibeat = 1;
                }
                if (WriteBeatEvent != null)
                    WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[start+1]));
            }
            else
            {
                if (WriteBeatEvent != null)
                    WriteBeatEvent(String.Format("m0 bt4:{0}", wordtext[start + 1]));
                iflag = 1;
            }
            List<int> _liner_dur2 = new List<int>();
            evaSinging_LinearScale(primaryDur[start + 1], _liner_dur2, halfbeat * 2);
            lineScarDur.Add(_liner_dur2);

            ibeat = ibeat + 1;
        }


        public static void strench_2beat4syllable(int start, int end, List<List<int>> primaryDur, List<List<int>> lineScarDur, ref int imeasure, ref double ibeat, int Gindex,List<string> wordtext,ref int iflag)
        {
            for (int ii = start; ii <= start + 1; ii++)
            {
                //write beat log
                if (Gindex != 0||iflag==1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii-1]));
                    ibeat = ibeat + 0.5;
                }
                else
                {
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m0 bt{0}:{1}", 4 - (3 - ii) * 0.5, wordtext[ii - 1]));
                   
                }

                List<int> _liner_dur11 = new List<int>();
                evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                lineScarDur.Add(_liner_dur11);             
            }

            for (int ii = start + 2; ii < end; ii++)
            {
                //write beat log
                if (Gindex != 0||iflag==1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]));
                    ibeat = ibeat + 0.5;
                }
                else
                {
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m0 bt{0}:{1}", 5 - (5 - ii) * 0.5, wordtext[ii - 1]));
                    ibeat = 5 - (5 - ii) * 0.5 + 0.5;
                    if(ii==end-1)
                        iflag = 1;

                }
                List<int> _liner_dur11 = new List<int>();
                evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                lineScarDur.Add(_liner_dur11);

            }
        }

        public static void strench_2beat5syllable(int start, int end, List<List<int>> primaryDur, List<List<int>> lineScarDur, ref int imeasure, ref double ibeat, int Gindex,List<string> wordtext)
        {
            for (int ii = start; ii <= start + 1; ii++)
            {
                if (imeasure == 0)
                {
                    imeasure = 1;
                    ibeat = 1;
                }
                else if (ibeat == 5)
                {
                    imeasure = imeasure + 1;
                    ibeat = 1;
                }
                if (WriteBeatEvent != null)
                    WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]));
           
                List<int> _liner_dur2 = new List<int>();
                evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                lineScarDur.Add(_liner_dur2);
                //add 0.5 beat,judge and get measure
                ibeat = ibeat + 0.5;

            }
            int diffbefore = end - start - 1;
            int totalAll = halfbeat * 2;
            int total = totalAll;
            int step = totalAll / (diffbefore - 1);
            double beatstep = Math.Round(1.0 / (diffbefore - 1), 2);
            double beattotal = 1.0;

            for (int _4non = start + 2; _4non < end; _4non++)
            {
                if (imeasure == 0)
                {
                    imeasure = 1;
                    ibeat = 1;
                }
                else if (ibeat == 5)
                {
                    imeasure = imeasure + 1;
                    ibeat = 1;
                }
                if (WriteBeatEvent != null)
                    WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[_4non - 1]));
               
                List<int> _liner_dur6 = new List<int>();
                if (_4non == end - 1)
                {
                    evaSinging_LinearScale(primaryDur[_4non - 1], _liner_dur6, total);
                    //add rest beat ,judge and get measure
                    ibeat = ibeat + beattotal;
                }

                else
                {
                  evaSinging_LinearScale(primaryDur[_4non - 1], _liner_dur6, step);
                    total = total - step;
                    //add step beat ,judge and get measure
                    ibeat = ibeat + beatstep;
                    beattotal = beattotal - beatstep;
                }
                lineScarDur.Add(_liner_dur6);
            }
        }

        public static void Align_stress_syllable_rule_new(List<List<int>> syllableStressIndex, List<int> syllableSeqindex, List<List<int>> phraseGroups, List<List<int>> primaryDur, List<List<int>> lineScarDur, List<List<int>> PrimarystressIndex, List<string> Syllable2phones, List<string> wordtext)
        {
            int imeasure = 0;
            double ibeat = 0;
            daddsil.Clear();

            int iflag = 0;
            for(int gindex=0;gindex<phraseGroups.Count;gindex++)
            {
                if (phraseGroups[gindex].Count == 1)
                    continue;
                if (syllableStressIndex[gindex].Count() != 0)
                {
                    //for stress in each group
                    for (int i = 0; i < syllableStressIndex[gindex].Count(); i++)
                    {
                        if(i==0)
                        {
                            int start = phraseGroups[gindex][0];
                            int end = syllableStressIndex[gindex][0];
                            int d = end - start;
                            if(d!=0)
                            {
                                switch(d)
                                {
                                    case 1:
                                        strench_1beat1syllable(start, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext,ref iflag);
                                        break;
                                    case 2:
                                        strench_1beat2syllable_0(start, end, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                        break;
                                    case 3:
                                        strench_2beat3syllable(start, end, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                        break;
                                    case 4:
                                        strench_2beat4syllable(start, end, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                        break;
                                    default:
                                        break;

                                }
                                
                            }
                           
                        }
                        else 
                        {
                            int startdex = syllableStressIndex[gindex][i - 1];
                            int enddex = syllableStressIndex[gindex][i];
                            int diffbefore = enddex - startdex;
                            if (diffbefore != 0)
                            {
                                switch (diffbefore)
                                {
                                    case 2:
                                        strench_1beat2syllable(startdex, enddex, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                        break;
                                    case 3:
                                        strench_2beat3syllable(startdex, enddex, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                        break;
                                    case 4:
                                        strench_2beat4syllable(startdex, enddex, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                        break;
                                    case 5:
                                        strench_2beat5syllable(startdex, enddex, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext);
                                        break;
                                    default:
                                        break;

                                }
                            }
                        }
                        if(i == syllableStressIndex[gindex].Count - 1)   //last stress syllable
                        {
                            List<int> _liner_dur7 = new List<int>();
                            int groupend = phraseGroups[gindex].Count - 1;

                            int start = syllableStressIndex[gindex][i];
                            int end = phraseGroups[gindex][groupend];
                            int lastdiff = end - start;
                            switch(lastdiff)
                            {
                                case 0:
                                    strench_1beat1syllable(start, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext, ref iflag);
                                    break;
                                case 1:
                                    strench_1beat2syllable_0(start, end+1,primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext,ref iflag);
                                    break;
                                case 2:
                                    strench_2beat3syllable(start, end + 1, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext,ref iflag);
                                    break;
                                case 3:
                                    strench_2beat4syllable(start, end + 1, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex,wordtext, ref iflag);
                                    break;
                                case 4:
                                    strench_2beat5syllable(start, end+1, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext);
                                    break;
                                default:
                                    break;
                            }
                        }
                        
                    }
                }
                else
                {
                    //if have not stress in group
                    int groupend = phraseGroups[gindex].Count - 1;
                    List<int> _liner_durG = new List<int>();
                    int start = phraseGroups[gindex][0];
                    int end = phraseGroups[gindex][groupend];
                    int diff = end - start;
                    switch(diff)
                    {
                        case 0:
                            strench_1beat1syllable(start, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex,wordtext,ref iflag);
                            break;
                        case 1:
                            strench_1beat2syllable(start, end + 1, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext,ref iflag);
                            break;
                        case 2:
                            strench_2beat3syllable(start, end + 1, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext,ref iflag);
                            break;
                        case 3:
                            strench_2beat4syllable(start, end + 1, primaryDur, lineScarDur, ref imeasure, ref ibeat, gindex, wordtext,ref iflag);
                            break;
                        case 4:
                            break;
                        default:
                            break;
                    }
                }

                combine2Groups(primaryDur, syllableSeqindex, lineScarDur, phraseGroups, syllableStressIndex, ref imeasure, ref ibeat, gindex, wordtext);
           }
        }

        public static void Align_stress_syllable_rule(List<List<int>>syllableStressIndex, List<int> syllableSeqindex, List<List<int>> phraseGroups, List<List<int>> primaryDur, List<List<int>> lineScarDur,List<List<int>> PrimarystressIndex,List<string>Syllable2phones,string filename,List<string>wordtext)
        {
            //List<int> syllablesOfGroup = new List<int>();
            //20170718
            
            string path = Path.Combine(@"E:\Work\workdir\Beat", filename);

             int imeasure = 0;
             double ibeat = 0;
            daddsil.Clear();

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                //for groups
                for (int gindex = 0; gindex < phraseGroups.Count; gindex++)
                {
                   
                 
                    if (phraseGroups[gindex].Count == 1)
                        continue;

                    if (syllableStressIndex[gindex].Count() != 0)
                    {
                        //for stress in each group
                        for (int i = 0; i < syllableStressIndex[gindex].Count(); i++)
                        {
                            if (i == 0)
                            {

                                int start = phraseGroups[gindex][i];
                                int end = syllableStressIndex[gindex][i];


                                if (start != end)
                                {
                                    int d = end - start;
                                    if (d == 1)
                                    {
                                        if(gindex != 0)
                                        {
                                            if (ibeat == 5)
                                            {
                                                imeasure= imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[start - 1]);
                                        }
                                        else
                                        {
                                            sw.WriteLine("m0 bt4:{0}", wordtext[start - 1]);
                                            
                                        }
                                        
                                        List<int> _liner_dur0 = new List<int>();
                                        evaSinging_LinearScale(primaryDur[start - 1], _liner_dur0, halfbeat * 2);
                                        lineScarDur.Add(_liner_dur0);

                                        //add 1beat,judge and get measure
                                        if (/*phonename != "0000000001"||*/ gindex != 0)
                                        {
                                           
                                                ibeat=ibeat+1;

                                            
                                        }
                                        else
                                        {
                                           
                                            ibeat = 5;
                                        }


                                    }
                                    else if (d == 2)
                                    {
                                        for (int ii = start; ii < end; ii++)
                                        {
                                            //write beat log
                                            if (gindex != 0)
                                            {
                                                if (ibeat == 5)
                                                {
                                                    imeasure= imeasure+1;
                                                    ibeat = 1;
                                                }
                                                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);
                                            }
                                            else
                                            {
                                                sw.WriteLine("m0 bt{0}:{1}", 5 - (3 - ii) * 0.5, wordtext[ii - 1]);
                                            }

                                            List<int> _liner_dur2 = new List<int>();
                                            evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                                            lineScarDur.Add(_liner_dur2);

                                            if (/*phonename != "0000000001" || */gindex != 0)
                                            {
                                                ibeat = ibeat + 0.5;
                                            
                                            }
                                            else
                                            {
                                                
                                                ibeat = 5 - (3 - ii) * 0.5 + 0.5;
                                            }


                                        }

                                       
                                    }
                                    else if (d == 3)
                                    {
                                       
                                        for (int ii = start; ii <= start+1; ii++)
                                        {
                                            //write beat log
                                            if (gindex != 0)
                                            {
                                                if (ibeat == 5)
                                                {
                                                    imeasure= imeasure+1;
                                                    ibeat = 1;
                                                }
                                                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);
                                            }
                                            else
                                            {
                                                sw.WriteLine("m0 bt{0}:{1}", 4 - (3 - ii) * 0.5, wordtext[ii - 1]);
                                            }
                                            List<int> _liner_dur11 = new List<int>();
                                            evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                                            lineScarDur.Add(_liner_dur11);

                                            if (/*phonename != "0000000001" ||*/ gindex != 0)
                                            {
                                                ibeat = ibeat + 0.5;
                                                
                                            }
                                            else
                                            {
                                               
                                                ibeat = 4 - (3 - ii) * 0.5 + 0.5;
                                            }


                                        }



                                        //write beat log
                                        if (gindex != 0)
                                        {
                                            if (ibeat == 5)
                                            {
                                                imeasure= imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[start + 1]);
                                        }
                                        else
                                        {
                                            sw.WriteLine("m0 bt4:{0}", wordtext[start + 1]);
                                        }
                                        List<int> _liner_dur2 = new List<int>();
                                        evaSinging_LinearScale(primaryDur[start + 1], _liner_dur2, halfbeat * 2);
                                        lineScarDur.Add(_liner_dur2);

                                       
                                        ibeat = ibeat + 1;
                                    }
                                    //else if (d == 3)
                                    //{
                                    //    //List<int> _liner_dur2 = new List<int>();
                                    //    //evaSinging_LinearScale(primaryDur[start - 1], _liner_dur2, halfbeat*2);
                                    //    //lineScarDur.Add(_liner_dur2);

                                    //    for (int ii = start; ii < end; ii++)
                                    //    {
                                    //        List<int> _liner_dur11 = new List<int>();
                                    //        evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                                    //        lineScarDur.Add(_liner_dur11);

                                    //        if (/*phonename != "0000000001" ||*/ gindex != 0)
                                    //        {
                                    //            if (imeasure == 0)
                                    //            {
                                    //                imeasure = 1;
                                    //                ibeat = 0.5;
                                    //            }
                                    //            else
                                    //            {
                                    //                ibeat = ibeat == 4 ? 0.5 : ibeat + 0.5;
                                    //                imeasure = ibeat == 0.5 ? imeasure + 1 : imeasure;
                                    //            }
                                    //            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, Syllable2phones[ii - 1]);
                                    //        }


                                    //    }

                                    //}
                                    //else if (d == 4)
                                    //{
                                    //    List<int> _liner_dur2 = new List<int>();
                                    //    evaSinging_LinearScale(primaryDur[start - 1], _liner_dur2, halfbeat);
                                    //    lineScarDur.Add(_liner_dur2);

                                    //    if (/*phonename != "0000000001" || */gindex != 0)
                                    //    {
                                    //        if (imeasure == 0)
                                    //        {
                                    //            imeasure = 1;
                                    //            ibeat = 0.5;
                                    //        }
                                    //        else
                                    //        {
                                    //            ibeat = ibeat == 4 ? 0.5 : ibeat + 0.5;
                                    //            imeasure = ibeat == 0.5 ? imeasure + 1 : imeasure;
                                    //        }
                                    //        sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, Syllable2phones[start - 1]);
                                    //    }

                                    //    int totalAll = halfbeat * 2;
                                    //    int total = totalAll;
                                    //    int step = totalAll / (d - 1);

                                    //    double beatstep = 1.0 / (d - 1);
                                    //    double beattotal = 1;

                                    //    for (int _4non = start + 1; _4non < end; _4non++)
                                    //    {
                                    //        List<int> _liner_dur22 = new List<int>();
                                    //        if (_4non == end - 1)
                                    //        {
                                    //            evaSinging_LinearScale(primaryDur[_4non - 1], _liner_dur22, total);

                                    //            if (gindex != 0)
                                    //            {
                                    //                if (imeasure == 0)
                                    //                {
                                    //                    imeasure = 1;
                                    //                    ibeat = beattotal;
                                    //                }
                                    //                else
                                    //                {
                                    //                    ibeat = ibeat == 4 ? beattotal : ibeat + beattotal;
                                    //                    imeasure = ibeat == beattotal ? imeasure + 1 : imeasure;
                                    //                }
                                    //                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, Syllable2phones[_4non - 1]);
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            evaSinging_LinearScale(primaryDur[_4non - 1], _liner_dur22, step);
                                    //            total = total - step;

                                    //            if (gindex != 0)
                                    //            {
                                    //                if (imeasure == 0)
                                    //                {
                                    //                    imeasure = 1;
                                    //                    ibeat = beatstep;
                                    //                }
                                    //                else
                                    //                {
                                    //                    ibeat = ibeat == 4 ? beatstep : ibeat + beatstep;
                                    //                    imeasure = ibeat == beatstep ? imeasure + 1 : imeasure;
                                    //                }
                                    //                beattotal = beattotal - beatstep;
                                    //                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, Syllable2phones[_4non - 1]);
                                    //            }
                                    //        }

                                    //        lineScarDur.Add(_liner_dur22);
                                    //    }

                                    //}
                                    else if (d == 4)
                                    {
                                        for (int ii = start; ii <= start+1; ii++)
                                        {
                                            //write beat log
                                            if (gindex != 0)
                                            {
                                                if (ibeat == 5)
                                                {
                                                    imeasure= imeasure+1;
                                                    ibeat = 1;
                                                }
                                                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);
                                            }
                                            else
                                            {
                                                sw.WriteLine("m0 bt{0}:{1}", 4 - (3 - ii) * 0.5, wordtext[ii - 1]);
                                            }

                                            List<int> _liner_dur11 = new List<int>();
                                            evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                                            lineScarDur.Add(_liner_dur11);

                                            if (gindex != 0)
                                            {
                                                ibeat = ibeat + 0.5;
                                              
                                            }
                                           
                                        }

                                        for (int ii = start+2; ii <end; ii++)
                                        {
                                            //write beat log
                                            if (gindex != 0)
                                            {
                                                if (ibeat == 5)
                                                {
                                                    imeasure= imeasure+1;
                                                    ibeat = 1;
                                                }
                                                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);
                                            }
                                            else
                                            {
                                                sw.WriteLine("m0 bt{0}:{1}", 5 - (5 - ii) * 0.5, wordtext[ii - 1]);
                                            }


                                            List<int> _liner_dur11 = new List<int>();
                                            evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur11, halfbeat);
                                            lineScarDur.Add(_liner_dur11);

                                            if (/*phonename != "0000000001" ||*/ gindex != 0)
                                            {
                                                ibeat = ibeat + 0.5;
                                               
                                            }
                                            else
                                            {
                                               
                                                ibeat = 5 - (5 - ii) * 0.5 + 0.5;
                                            }


                                        }

                                    }
                                    else
                                    {
                                        //722
                                        align_syllable_with_stress(start, end, primaryDur, lineScarDur, PrimarystressIndex[gindex], syllableSeqindex, ref imeasure, ref ibeat);

                                    }



                                }                               

                            }
                            else
                            {
                                int diffbefore = syllableStressIndex[gindex][i] - syllableStressIndex[gindex][i - 1] - 1; //non-syllable numbers
                                if (diffbefore == 0)
                                {
                                    List<int> _liner_dur3 = new List<int>();
                                    int idex = syllableStressIndex[gindex][i - 1] - 1;
                                    evaSinging_LinearScale(primaryDur[idex], _liner_dur3, halfbeat * 2);
                                    lineScarDur.Add(_liner_dur3);

                                    // int lastsyllableinG = phraseGroups[gindex].Count - 1;
                                    int value = syllableStressIndex[gindex][i];
                                    int sil = syllableSeqindex.IndexOf(value);
                                    syllableSeqindex.Insert(sil, 0);


                                }
                                else if (diffbefore == 1)
                                {

                                    for (int ii = syllableStressIndex[gindex][i - 1]; ii < syllableStressIndex[gindex][i]; ii++)
                                    {
                                        //write beat log                                  
                                        if (imeasure == 0)
                                        {
                                            imeasure = 1;
                                            ibeat = 1;
                                        }
                                        else if (ibeat == 5)
                                            {
                                                imeasure = imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);
                                       

                                        List<int> _liner_dur4 = new List<int>();
                                        evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur4, halfbeat * 2);
                                        lineScarDur.Add(_liner_dur4);

                                        //add 1beat,judge and get measure
                                        ibeat = ibeat+1;
                                       
                                    }


                                }
                                else if (diffbefore == 2)
                                {
                                    if (imeasure == 0)
                                    {
                                        imeasure = 1;
                                        ibeat = 1;
                                    }
                                    else if (ibeat == 5)
                                    {
                                        imeasure = imeasure+1;
                                        ibeat = 1;
                                    }
                                    sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[syllableStressIndex[gindex][i - 1] - 1]);

                                    List<int> _liner_dur5 = new List<int>();
                                    evaSinging_LinearScale(primaryDur[syllableStressIndex[gindex][i - 1] - 1], _liner_dur5, halfbeat * 2);
                                    lineScarDur.Add(_liner_dur5);

                                    //add 1beat,judge and get measure
                                    ibeat= ibeat+1;
                                    
                                    for (int ii = syllableStressIndex[gindex][i - 1] + 1; ii < syllableStressIndex[gindex][i]; ii++)
                                    {
                                        if (imeasure == 0)
                                        {
                                            imeasure = 1;
                                            ibeat = 1;
                                        }
                                        else if (ibeat == 5)
                                        {
                                            imeasure= imeasure+2;
                                            ibeat = 1;
                                        }
                                        sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);


                                        List<int> _liner_dur2 = new List<int>();
                                        evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                                        lineScarDur.Add(_liner_dur2);
                                        //add 0.5 beat,judge and get measure
                                        ibeat = ibeat + 0.5;
                                       
                                    }
                                }
                                else if (diffbefore == 4 || diffbefore == 3)
                                {
                                    for (int ii = syllableStressIndex[gindex][i - 1]; ii <= syllableStressIndex[gindex][i - 1] + 1; ii++)
                                    {
                                        if (imeasure == 0)
                                        {
                                            imeasure = 1;
                                            ibeat = 1;
                                        }
                                        else if(ibeat == 5)
                                        {
                                            imeasure= imeasure+2;
                                            ibeat = 1;
                                        }
                                        sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);


                                        List<int> _liner_dur2 = new List<int>();
                                        evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                                        lineScarDur.Add(_liner_dur2);
                                        //add 0.5 beat,judge and get measure
                                        ibeat = ibeat + 0.5;
                                      
                                    }

                                    int totalAll = halfbeat * 2;
                                    int total = totalAll;
                                    int step = totalAll / (diffbefore - 1);
                                    double beatstep = Math.Round(1.0 / (diffbefore - 1),2);
                                    double beattotal = 1.0;

                                    for (int _4non = syllableStressIndex[gindex][i - 1] + 2; _4non < syllableStressIndex[gindex][i]; _4non++)
                                    {
                                        if (imeasure == 0)
                                        {
                                            imeasure = 1;
                                            ibeat = 1;
                                        }
                                        else if (ibeat == 5)
                                        {
                                            imeasure= imeasure+1;
                                            ibeat = 1;
                                        }
                                        sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[_4non - 1]);

                                        List<int> _liner_dur6 = new List<int>();
                                        if (_4non == syllableStressIndex[gindex][i] - 1)
                                        {
                                            evaSinging_LinearScale(primaryDur[_4non - 1], _liner_dur6, total);
                                            //add rest beat ,judge and get measure
                                            ibeat=ibeat+beattotal;
                                           
                                        }

                                        else
                                        {
                                     

                                            evaSinging_LinearScale(primaryDur[_4non - 1], _liner_dur6, step);
                                            total = total - step;
                                            //add step beat ,judge and get measure
                                            ibeat = ibeat + beatstep;
                                            beattotal = beattotal - beatstep;
                                           
                                        }

                                        lineScarDur.Add(_liner_dur6);
                                    }

                                }
                                else
                                {
                                    //722

                                    align_syllable_with_stress(syllableStressIndex[gindex][i - 1], syllableStressIndex[gindex][i], primaryDur, lineScarDur, PrimarystressIndex[gindex], syllableSeqindex, ref imeasure, ref ibeat);

                                }
                            }
                            //last syllable in groups
                            if (i == syllableStressIndex[gindex].Count() - 1)
                            {
                                List<int> _liner_dur7 = new List<int>();
                                int groupend = phraseGroups[gindex].Count - 1;

                                int start = syllableStressIndex[gindex][i];
                                int end = phraseGroups[gindex][groupend];
                                if (start == end)
                                {
                                    if (imeasure == 0)
                                    {
                                        imeasure = 1;
                                        ibeat = 1;
                                    }
                                    else if (ibeat == 5)
                                    {
                                        imeasure = imeasure+1;
                                        ibeat = 1;
                                    }
                                    sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[start - 1]);

                                    evaSinging_LinearScale(primaryDur[start - 1], _liner_dur7, halfbeat * 2);
                                    lineScarDur.Add(_liner_dur7);

                                    ibeat = ibeat + 1;
                                   
                                }
                                else
                                {

                                    int d = end - start;
                                    if (d == 1)
                                    {

                                        for (int dex = start; dex <= end; dex++)
                                        {
                                            if (imeasure == 0)
                                            {
                                                imeasure = 1;
                                                ibeat = 1;
                                            }
                                            else if (ibeat == 5)
                                            {
                                                imeasure = imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[dex - 1]);

                                            List<int> _liner_dur_dex = new List<int>();
                                            evaSinging_LinearScale(primaryDur[dex - 1], _liner_dur_dex, halfbeat);
                                            lineScarDur.Add(_liner_dur_dex);

                                            ibeat = ibeat + 0.5;
                                           
                                        }

                                    }

                                    else if (d == 2)
                                    {


                                        for (int ii = start; ii <= start + 1; ii++)
                                        {

                                            if (imeasure == 0)
                                            {
                                                imeasure = 1;
                                                ibeat = 1;
                                            }
                                            else if (ibeat == 5)
                                            {
                                                imeasure = imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);

                                            List<int> _liner_dur22 = new List<int>();
                                            evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur22, halfbeat);
                                            lineScarDur.Add(_liner_dur22);

                                            ibeat = ibeat + 0.5;
                                           
                                        }

                                        if (imeasure == 0)
                                        {
                                            imeasure = 1;
                                            ibeat = 1;
                                        }
                                        else if (ibeat == 5)
                                        {
                                            imeasure = imeasure+1;
                                            ibeat = 1;
                                        }
                                        sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[end - 1]);

                                        List<int> _liner_dur2 = new List<int>();
                                        evaSinging_LinearScale(primaryDur[end - 1], _liner_dur2, halfbeat * 2);
                                        lineScarDur.Add(_liner_dur2);

                                        ibeat = ibeat + 1;
                                       

                                    }
                                    else if (d == 3 || d == 4)
                                    {
                                        for (int ii = start; ii <= start + 1; ii++)
                                        {
                                            if (imeasure == 0)
                                            {
                                                imeasure = 1;
                                                ibeat = 1;
                                            }
                                            else if (ibeat == 5)
                                            {
                                                imeasure = imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);


                                            List<int> _liner_dur2 = new List<int>();
                                            evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                                            lineScarDur.Add(_liner_dur2);

                                            ibeat = ibeat + 0.5;

                                         
                                        }

                                        int totalAll = halfbeat * 2;
                                        int total = totalAll;
                                        int step = totalAll / (d - 1);
                                        double beatstep = Math.Round(1.0 / (d - 1),2);
                                        double beattotal = 1.0;

                                        for (int _4non = start + 2; _4non <= end; _4non++)
                                        {
                                            if (imeasure == 0)
                                            {
                                                imeasure = 1;
                                                ibeat = 1;
                                            }
                                            else if (ibeat == 5)
                                            {
                                                imeasure = imeasure+1;
                                                ibeat = 1;
                                            }
                                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[_4non - 1]);


                                            List<int> _liner_durlas = new List<int>();
                                            if (_4non == end)
                                            {
                                                evaSinging_LinearScale(primaryDur[_4non - 1], _liner_durlas, total);
                                                //add rest beat
                                                ibeat = ibeat + beattotal;
                                                
                                            }

                                            else
                                            {
                                                evaSinging_LinearScale(primaryDur[_4non - 1], _liner_durlas, step);
                                                total = total - step;
                                                //add step beat
                                                ibeat = ibeat + beatstep;
                                                beattotal = beattotal - beatstep;
                                                
                                            }

                                            lineScarDur.Add(_liner_durlas);
                                        }

                                    }
                                    else
                                    {
                                        //722
                                        align_syllable_with_stress(start, end + 1, primaryDur, lineScarDur, PrimarystressIndex[gindex], syllableSeqindex, ref imeasure, ref ibeat);
                                    }

                                }


                            }

                        }
                    }
                    else
                    {
                        //if have not stress in group
                        int groupend = phraseGroups[gindex].Count - 1;
                        List<int> _liner_durG = new List<int>();
                        int start = phraseGroups[gindex][0];
                        int end = phraseGroups[gindex][groupend];
                        if (start == end)
                        {

                            if (imeasure == 0)
                            {
                                imeasure = 1;
                                ibeat = 1;
                            }
                            else if (ibeat == 5)
                            {
                                imeasure = imeasure+1;
                                ibeat = 1;
                            }
                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[start - 1]);

                            evaSinging_LinearScale(primaryDur[start - 1], _liner_durG, halfbeat * 2);
                            lineScarDur.Add(_liner_durG);
                            //add 1 beat
                            ibeat = ibeat + 1;
                          
                        }
                        else
                        {

                            int d = end - start;
                            if (d == 1)
                            {

                                for (int dex = start; dex <= end; dex++)
                                {
                                    if (imeasure == 0)
                                    {
                                        imeasure = 1;
                                        ibeat = 1;
                                    }
                                    else if (ibeat == 5)
                                    {
                                        imeasure = imeasure+1;
                                        ibeat = 1;
                                    }
                                    sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[dex - 1]);


                                    List<int> _liner_dur_dexG = new List<int>();
                                    evaSinging_LinearScale(primaryDur[dex - 1], _liner_dur_dexG, halfbeat);
                                    lineScarDur.Add(_liner_dur_dexG);

                                    //add 0.5beat
                                    ibeat = ibeat + 0.5;
                                   

                                }

                            }
                            else if(d == 2)
                            {//722
                                for (int ii = start; ii <= start + 1; ii++)
                                {
                                    if (imeasure == 0)
                                    {
                                        imeasure = 1;
                                        ibeat = 1;
                                    }
                                    else if (ibeat == 5)
                                    {
                                        imeasure = imeasure+1;
                                        ibeat = 1;
                                    }
                                    sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);


                                    List<int> _liner_dur22 = new List<int>();
                                    evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur22, halfbeat);
                                    lineScarDur.Add(_liner_dur22);

                                    ibeat = ibeat + 0.5;
                                    
                                }

                                if (imeasure == 0)
                                {
                                    imeasure = 1;
                                    ibeat = 1;
                                }
                                else if (ibeat == 5)
                                {
                                    imeasure = imeasure+1;
                                    ibeat = 1;
                                }
                                sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[end - 1]);


                                List<int> _liner_dur2 = new List<int>();
                                evaSinging_LinearScale(primaryDur[end - 1], _liner_dur2, halfbeat * 2);
                                lineScarDur.Add(_liner_dur2);

                                ibeat = ibeat + 1;
                               
                            }
                            else if(d==3||d==4)
                            {
                                for (int ii = start; ii <= start + 1; ii++)
                                {
                                    if (imeasure == 0)
                                    {
                                        imeasure = 1;
                                        ibeat = 1;
                                    }
                                    else if (ibeat == 5)
                                    {
                                        imeasure = imeasure+1;
                                        ibeat = 1;
                                    }
                                    sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[ii - 1]);


                                    List<int> _liner_dur2 = new List<int>();
                                    evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur2, halfbeat);
                                    lineScarDur.Add(_liner_dur2);

                                    ibeat = ibeat + 0.5;
                                  
                                }

                                int totalAll = halfbeat * 2;
                                int total = totalAll;
                                int step = totalAll / (d - 1);
                                double beatstep =Math.Round(1.0 / (d - 1),2);
                                double beattotal = 1.0;

                                for (int _4non = start + 2; _4non <= end; _4non++)
                                {
                                    if (imeasure == 0)
                                    {
                                        imeasure = 1;
                                        ibeat = 1;
                                    }
                                    else if (ibeat == 5)
                                    {
                                        imeasure = imeasure+1;
                                        ibeat = 1;
                                    }
                                    sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[_4non - 1]);


                                    List<int> _liner_durlas = new List<int>();
                                    if (_4non == end)
                                    {
                                        evaSinging_LinearScale(primaryDur[_4non - 1], _liner_durlas, total);
                                        //add rest beat
                                        ibeat = ibeat + beattotal;
                                        
                                    }

                                    else
                                    {
                                        evaSinging_LinearScale(primaryDur[_4non - 1], _liner_durlas, step);
                                        total = total - step;
                                        //add step beat
                                        ibeat = ibeat + beatstep;
                                       
                                    }

                                    lineScarDur.Add(_liner_durlas);
                                }
                            }
                            else
                            {

                            }

                        }
                    }





                    //combine two phrase by correct measure
                    double siladd = 0;
                    double lastmesure = 0;
                    int startnext = 0;
                    int endnext = 0;
                    if (gindex == phraseGroups.Count - 1) continue;
                    if (phraseGroups[gindex + 1].Count == 1)
                    {
                        int value = phraseGroups[gindex + 2][0];

                        startnext = phraseGroups[gindex + 2][0];
                        if (syllableStressIndex[gindex + 2].Count != 0)
                            endnext = syllableStressIndex[gindex + 2][0];
                        else
                            endnext = startnext;
                        if (endnext - startnext == 1 || endnext - startnext == 2)
                        {
                            lastmesure = 1;
                        }
                        else if (endnext - startnext == 3 || endnext - startnext == 4)
                        {
                            lastmesure = 2;
                        }
                        else if (endnext - startnext >= 5)
                            lastmesure = 4;

                    }
                    else
                    {
                        int value = phraseGroups[gindex + 1][0];

                        startnext = phraseGroups[gindex + 1][0];
                        if (syllableStressIndex[gindex + 1].Count != 0)
                            endnext = syllableStressIndex[gindex + 1][0];
                        else
                            endnext = startnext;
                        if (endnext - startnext == 1 || endnext - startnext == 2)
                        {
                            lastmesure = 1;
                        }
                        else if (endnext - startnext == 3 || endnext - startnext == 4)
                        {
                            lastmesure = 2;
                        }
                        else if (endnext - startnext >= 5)
                            lastmesure = 4;
                    }

                    siladd = 8 - lastmesure - (ibeat-1);
                    siladd = siladd >= 4 ? siladd - 4 : siladd;
                    siladd = siladd >= 2 ? siladd - 2 : siladd;

                  
                    //deal with the next phrase which have one syllable
                    if (siladd != 0)
                    {
                        if (phraseGroups[gindex + 1].Count == 1)
                        {
                            if (ibeat == 5)
                            {
                                imeasure = imeasure+1;
                                ibeat = 1;
                            }
                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[phraseGroups[gindex + 1][0] - 1]);


                            List<int> _liner_dur = new List<int>();
                            evaSinging_LinearScale(primaryDur[phraseGroups[gindex + 1][0] - 1], _liner_dur, (int)(siladd * 2 * halfbeat));
                            lineScarDur.Add(_liner_dur);
                            ibeat = ibeat + siladd;
                           
                        }
                        else
                        {
                            int lastsyllableinG = phraseGroups[gindex].Count - 1;
                            int value = phraseGroups[gindex][lastsyllableinG];
                            int sil = syllableSeqindex.IndexOf(value);
                            syllableSeqindex.Insert(sil + 1, 0);
                            daddsil.Add(siladd);

                            if (ibeat == 5)
                            {
                                imeasure = imeasure+1;
                                ibeat = 1;
                            }
                            sw.WriteLine("m{0} bt{1}:sil", imeasure, ibeat);

                            ibeat = ibeat + siladd;
                          
                        }

                    }
                    else
                    {
                        if (phraseGroups[gindex + 1].Count == 1)
                        {
                            if (ibeat == 5)
                            {
                                imeasure = imeasure+1;
                                ibeat = 1;
                            }
                            sw.WriteLine("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[phraseGroups[gindex + 1][0] - 1]);

                            List<int> _liner_dur = new List<int>();
                            evaSinging_LinearScale(primaryDur[phraseGroups[gindex + 1][0] - 1], _liner_dur, 2 * 2 * halfbeat);
                            lineScarDur.Add(_liner_dur);

                            if(ibeat==4)
                            {
                                imeasure = imeasure + 1;
                                ibeat = 2;
                            }
                            else
                            {
                                ibeat = ibeat + 2;
                            }
                            
                            
                        }
                       
                    }
                   
                }
            }        

        }

        public static void combine2Groups(List<List<int>> primaryDur,List<int> syllableSeqindex, List<List<int>> lineScarDur, List<List<int>> phraseGroups,List<List<int>> syllableStressIndex, ref int imeasure, ref double ibeat, int Gindex,List<string>wordtext)
        {
            double siladd = 0;
            double lastmesure = 0;
            int startnext = 0;
            int endnext = 0;
            if (Gindex == phraseGroups.Count - 1)
                return;
            if (phraseGroups[Gindex + 1].Count == 1)
            {
                int value = phraseGroups[Gindex + 2][0];

                startnext = phraseGroups[Gindex + 2][0];
                if (syllableStressIndex[Gindex + 2].Count != 0)
                    endnext = syllableStressIndex[Gindex + 2][0];
                else
                    endnext = startnext;
                if (endnext - startnext == 1 || endnext - startnext == 2)
                {
                    lastmesure = 1;
                }
                else if (endnext - startnext == 3 || endnext - startnext == 4)
                {
                    lastmesure = 2;
                }
                else if (endnext - startnext >= 5)
                    lastmesure = 4;

            }
            else
            {
                int value = phraseGroups[Gindex + 1][0];

                startnext = phraseGroups[Gindex + 1][0];
                if (syllableStressIndex[Gindex + 1].Count != 0)
                    endnext = syllableStressIndex[Gindex + 1][0];
                else
                    endnext = startnext;
                if (endnext - startnext == 1 || endnext - startnext == 2)
                {
                    lastmesure = 1;
                }
                else if (endnext - startnext == 3 || endnext - startnext == 4)
                {
                    lastmesure = 2;
                }
                else if (endnext - startnext >= 5)
                    lastmesure = 4;
            }

            siladd = 8 - lastmesure - (ibeat - 1);
            siladd = siladd >= 4 ? siladd - 4 : siladd;
            siladd = siladd >= 2 ? siladd - 2 : siladd;


            //deal with the next phrase which have one syllable
            if (siladd != 0)
            {
                if (phraseGroups[Gindex + 1].Count == 1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[phraseGroups[Gindex + 1][0] - 1]));

                   

                    List<int> _liner_dur = new List<int>();
                    evaSinging_LinearScale(primaryDur[phraseGroups[Gindex + 1][0] - 1], _liner_dur, (int)(siladd * 2 * halfbeat));
                    lineScarDur.Add(_liner_dur);
                    ibeat = ibeat + siladd;

                }
                else
                {
                    int lastsyllableinG = phraseGroups[Gindex].Count - 1;
                    int value = phraseGroups[Gindex][lastsyllableinG];
                    int sil = syllableSeqindex.IndexOf(value);
                    syllableSeqindex.Insert(sil + 1, 0);
                    daddsil.Add(siladd);

                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:sil", imeasure, ibeat));
                    ibeat = ibeat + siladd;

                }

            }
            else
            {
                if (phraseGroups[Gindex + 1].Count == 1)
                {
                    if (ibeat == 5)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 1;
                    }
                    if (WriteBeatEvent != null)
                        WriteBeatEvent(String.Format("m{0} bt{1}:{2}", imeasure, ibeat, wordtext[phraseGroups[Gindex + 1][0] - 1]));

                    List<int> _liner_dur = new List<int>();
                    evaSinging_LinearScale(primaryDur[phraseGroups[Gindex + 1][0] - 1], _liner_dur, 2 * 2 * halfbeat);
                    lineScarDur.Add(_liner_dur);

                    if (ibeat == 4)
                    {
                        imeasure = imeasure + 1;
                        ibeat = 2;
                    }
                    else
                    {
                        ibeat = ibeat + 2;
                    }


                }

            }
        }
        //722when we do not find v-n accent,we should use stress
        public static void align_syllable_with_stress(int start, int end, List<List<int>> primaryDur, List<List<int>> lineScarDur, List<int> primarystress, List<int> syllableSeqindex,ref int imeasure,ref double ibeat)
        {
            
            int mid_stress = 0;
           
            List<int> midlist = new List<int>();
            //get stress between start and end
            List<int> stresslistnew = new List<int>();
            foreach (int istressdex in primarystress)
            {
                if (istressdex >= start && istressdex <= end)
                    stresslistnew.Add(istressdex);
            }

            double middvalue= start + (end - start-1) /2.0;
            List<double> abs = new List<double>();
            foreach (int value in stresslistnew)
            {
                abs.Add(Math.Abs(value - middvalue));
            }
            double min = abs.Min();

            int dex = 0;
            int bflag = 0;
            int dex1 = abs.IndexOf(min);
            for(int i=dex1+1;i<abs.Count;i++)
            {
                if(abs[i]==min)
                {
                    dex = i;
                    bflag = 1;
                }
            }
            if (bflag != 1)
                dex = dex1;
            mid_stress = stresslistnew[dex];

            //deal with start stress to mid stress,difference 1??
            if (mid_stress-start-1==0)
            {
                List<int> _liner = new List<int>();
                evaSinging_LinearScale(primaryDur[start - 1], _liner, halfbeat * 2);
                lineScarDur.Add(_liner);

                //add 1 beat
                if (imeasure == 0)
                {
                    imeasure = 1;
                    ibeat = 1;
                }
                else
                {
                    ibeat = ibeat == 4 ? 1 : ibeat + 1;
                    imeasure = ibeat == 1 ? imeasure + 1 : imeasure;
                }

                int sil = syllableSeqindex.IndexOf(start);
                syllableSeqindex.Insert(sil, 0);
            }
            else if(mid_stress - start - 1==1)
            {
                for (int ii = start; ii < mid_stress; ii++)
                {
                    List<int> _liner_dur = new List<int>();
                    evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur, halfbeat * 2);
                    lineScarDur.Add(_liner_dur);

                    //add 1 beat
                    if (imeasure == 0)
                    {
                        imeasure = 1;
                        ibeat = 1;
                    }
                    else
                    {
                        ibeat = ibeat == 4 ? 1 : ibeat + 1;
                        imeasure = ibeat == 1 ? imeasure + 1 : imeasure;
                    }

                }
            }
            else
            {
                judge_non_stress_group(start, mid_stress, primaryDur, lineScarDur, 4,ref imeasure,ref ibeat);
            }

            //deal with mid stress to end
            if(end-mid_stress==0)
            {

            }
            else if (end - mid_stress - 1 == 0)
            {
                List<int> _liner1 = new List<int>();
                evaSinging_LinearScale(primaryDur[mid_stress - 1], _liner1, halfbeat * 2);
                lineScarDur.Add(_liner1);

                int sil1 = syllableSeqindex.IndexOf(mid_stress);
                syllableSeqindex.Insert(sil1, 0);
            }
            else if (end - mid_stress - 1 == 1)
            {
                for (int ii = mid_stress; ii < end; ii++)
                {
                    List<int> _liner_dur = new List<int>();
                    evaSinging_LinearScale(primaryDur[ii - 1], _liner_dur, halfbeat * 2);
                    lineScarDur.Add(_liner_dur);

                    //add 1 beat
                    if (imeasure == 0)
                    {
                        imeasure = 1;
                        ibeat = 1;
                    }
                    else
                    {
                        ibeat = ibeat == 4 ? 1 : ibeat + 1;
                        imeasure = ibeat == 1 ? imeasure + 1 : imeasure;
                    }
                }
            }
            else
            {
                judge_non_stress_group(mid_stress, end, primaryDur, lineScarDur, 4,ref imeasure,ref ibeat);
            }



        }

        public static void judge_non_stress_group(int start, int end, List<List<int>> primaryDur, List<List<int>> lineScarDur,int number, ref int imeasure, ref double ibeat)
        {
            if((end-start)<=number)
            {
                int total = halfbeat * number;
                for (int i = start; i < end; i++)
                {
                    List<int> _liner = new List<int>();
                    if(i==end-1)
                    {
                        evaSinging_LinearScale(primaryDur[i - 1], _liner, total);
                    }    
                    else
                    {
                        evaSinging_LinearScale(primaryDur[i - 1], _liner, halfbeat);
                        total = total - halfbeat;
                    }
                        
                    lineScarDur.Add(_liner);

                }
            }
            else
            {


                int totalAll = halfbeat * number;
                int total = totalAll;
                int step = totalAll / (end - start);
                for (int i = start; i < end; i++)
                {
                    List<int> _liner_dur = new List<int>();
                    if (i == end - 1)
                    {
                        evaSinging_LinearScale(primaryDur[i - 1], _liner_dur, total);
                    }

                    else
                    {
                        evaSinging_LinearScale(primaryDur[i - 1], _liner_dur, step);
                        total = total - step;
                    }

                    lineScarDur.Add(_liner_dur);
                }
            }
            
            //int start_inGroup = 0;
            //int end_inGroup = 0;
            //int iflag_find = 0;
            //for (int m = 0; m < phraseGroups.Count(); m++)
            //{
            //    if (phraseGroups[m].Contains(start))
            //        start_inGroup = m;
            //    if (phraseGroups[m].Contains(end))
            //        end_inGroup = m;
            //}
            //if (start_inGroup == end_inGroup)
            //{
            //    int indexay = syllableSeqindex.IndexOf(end);
            //    syllableSeqindex.Insert(indexay, 0);
            //}
            //else
            //{
            //    for (int data = start + 1; data < end; data++)
            //    {
            //        if (phraseGroups[end_inGroup].Contains(data))
            //        {
            //            int index = syllableSeqindex.IndexOf(data);
            //            syllableSeqindex.Insert(index, 0);
            //            iflag_find = 1;
            //            break;
            //        }

            //    }
            //    if (iflag_find == 0)
            //        syllableSeqindex.Insert(end, 0);

            //}



        }
        //spile list to stress and non stress list
        public static void splite_stressAndNonstressList(List<int> stressList, List<List<int>> syllableStressIndex, List<int> syllableSeqIndex,List<List<int>>Groups)
        {
            for(int iGdex=0;iGdex<Groups.Count;iGdex++)
            {
                List<int> istressGroup = new List<int>();
                for(int j=0;j<Groups[iGdex].Count;j++)
                {
                    int aaaaa = Groups[iGdex][j];
                    if (stressList[Groups[iGdex][j]-1] == 1)
                    {
                        if(istressGroup.Count== 0)
                            istressGroup.Add(Groups[iGdex][j]);
                        else
                        {
                            if(istressGroup[istressGroup.Count-1]!= Groups[iGdex][j]-1)
                                istressGroup.Add(Groups[iGdex][j]);

                        }
                    }
                        
                }
                syllableStressIndex.Add(istressGroup);
            }
            for (int i = 0; i < stressList.Count(); i++)
            {
                syllableSeqIndex.Add(i + 1);
            }
        }
        public static void find_stress_beatween_continous_5nonstress(List<List<int>> syllableStressVN, List<List<int>> syllablestressPrimary, List<List<int>> groupsPhrase)
        {
            for (int iGdex = 0; iGdex < syllableStressVN.Count; iGdex++)
            {
                List<int> istressGroup = new List<int>();
                for (int j = 0; j < syllableStressVN[iGdex].Count; j++)
                {
                    int mid_stress = 0;
                    List<int> midlist = new List<int>();
                    //get stress between start and end
                    List<int> stresslistnew = new List<int>();
                    int start = 0;
                    int end = 0;

                    if (j == 0)
                    {
                            //deal with non-stress number >5
                       start = groupsPhrase[iGdex][0];
                       end = syllableStressVN[iGdex][0];
                       if(end-start>=5)
                        {
                            foreach (int istressdex in syllablestressPrimary[iGdex])
                            {
                                if (istressdex >= start && istressdex <= end)
                                {
                                    if (istressdex == end - 1)
                                        continue;
                                    else
                                        stresslistnew.Add(istressdex);
                                }

                            }

                            double middvalue = start + (end - start) / 2.0;
                            List<double> abs = new List<double>();
                            foreach (int value in stresslistnew)
                            {
                                abs.Add(Math.Abs(value - middvalue));
                            }
                            double min = abs.Min();
                            int dex1 = abs.IndexOf(min);
                            mid_stress = stresslistnew[dex1];
                            //modify the mid syllable to stress
                            if (mid_stress < syllableStressVN[iGdex][0])
                                syllableStressVN[iGdex].Insert(0, mid_stress);
                            else
                                syllableStressVN[iGdex].Insert(1, mid_stress);                                             
                          
                        }
                            
                        
                    }
                    else
                    {
                        start = syllableStressVN[iGdex][j - 1];
                        end = syllableStressVN[iGdex][j];
                        //deal with non-stress number >5
                        if (end - start > 5)
                        {
                            foreach (int istressdex in syllablestressPrimary[iGdex])
                            {
                                if (istressdex >= start && istressdex <= end)
                                {
                                    if (istressdex == start + 1 || istressdex == end - 1)
                                        continue;
                                    else
                                        stresslistnew.Add(istressdex);
                                }

                            }

                            double middvalue = start + (end - start) / 2.0;
                            List<double> abs = new List<double>();
                            foreach (int value in stresslistnew)
                            {
                                abs.Add(Math.Abs(value - middvalue));
                            }
                            double min = abs.Min();
                            int dex1 = abs.IndexOf(min);
                            mid_stress = stresslistnew[dex1];
                            //modify the mid syllable to stress
                            if (mid_stress < start)
                                syllableStressVN[iGdex].Insert(j - 1, mid_stress);
                            else if (mid_stress > start && mid_stress < end)
                                syllableStressVN[iGdex].Insert(j, mid_stress);
                            else
                                syllableStressVN[iGdex].Insert(j + 1, mid_stress);                                                 
                                //for (int ivalue = 0; ivalue < syllableStressVN[iGdex].Count; ivalue++)
                                //{
                                //    if (syllableStressVN[iGdex][ivalue] < mid_stress && mid_stress < syllableStressVN[iGdex][ivalue + 1])
                                //        syllableStressVN[iGdex].Insert(ivalue + 1, mid_stress);
                                //}
                         
                               
                            
                           

                        }

                    }
                    if(j==syllableStressVN[iGdex].Count-1)
                    {
                        end = groupsPhrase[iGdex][groupsPhrase[iGdex].Count-1];
                        start = syllableStressVN[iGdex][j];
                        if (end - start >= 5)
                        {
                            foreach (int istressdex in syllablestressPrimary[iGdex])
                            {
                                if (istressdex >= start && istressdex <= end)
                                {
                                    if (istressdex == end - 1)
                                        continue;
                                    else
                                        stresslistnew.Add(istressdex);
                                }

                            }

                            double middvalue = start + (end - start) / 2.0;
                            List<double> abs = new List<double>();
                            foreach (int value in stresslistnew)
                            {
                                abs.Add(Math.Abs(value - middvalue));
                            }
                            double min = abs.Min();
                            int dex1 = abs.IndexOf(min);
                            mid_stress = stresslistnew[dex1];
                            //modify the mid syllable to stress
                            syllableStressVN[iGdex].Insert(j + 1, mid_stress);
                                //for (int ivalue = 0; ivalue < syllableStressVN[iGdex].Count; ivalue++)
                                //{
                                //    if (syllableStressVN[iGdex][ivalue] < mid_stress && mid_stress < syllableStressVN[iGdex][ivalue + 1])
                                //        syllableStressVN[iGdex].Insert(ivalue + 1, mid_stress);
                                //}
                          
                               
                        }


                    }


                }

                }

            }
        

        //add sil
        public static void nominizetion_phoneseq(List<int> syllableSeqindex, List<List<string>> syllable2phones, List<List<int>> phonedurs2line_List)
        {
            List<string> addsil = new List<string>();
            addsil.Add("sil");
            
            int index = 0;
            for (int i = 0; i < syllableSeqindex.Count(); i++)
            {
                if (syllableSeqindex[i] == 0)
                {
                    List<int> intsil = new List<int>();
                    syllable2phones.Insert(i, addsil);
                    intsil.Add((int)(daddsil[index]*2*halfbeat));
                    phonedurs2line_List.Insert(i, intsil);
                    index++;
                }


            }
        }




    }
}
