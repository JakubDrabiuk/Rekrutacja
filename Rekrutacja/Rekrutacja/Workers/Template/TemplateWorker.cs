using Soneta.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soneta.Kadry;
using Soneta.KadryPlace;
using Soneta.Types;
using Rekrutacja.Workers.Template;

//Rejetracja Workera - Pierwszy TypeOf określa jakiego typu ma być wyświetlany Worker, Drugi parametr wskazuje na jakim Typie obiektów będzie wyświetlany Worker
[assembly: Worker(typeof(TemplateWorker), typeof(Pracownicy))]
namespace Rekrutacja.Workers.Template
{
    public class TemplateWorker
    {
        //Aby parametry działały prawidłowo dziedziczymy po klasie ContextBase
        public class TemplateWorkerParametry : ContextBase
        {

            // Parametry do zadania 3 
/*
            [Caption("A")]
            public string ZmiennaA { get; set; }
            [Caption("B")]
            public string ZmiennaB { get; set; }*/


            [Caption("Data obliczeń")]
            public Date DataObliczen { get; set; }
            [Caption("A")]
            public int ZmiennaA { get; set; }
            [Caption("B")]
            public int ZmiennaB { get; set; }
            [Caption("operacja")]
            public string Operacja { get; set; }
            [Caption("Figura")]
            public Figura figura { get; set; } = new Figura();// Parametr potrzebny do zadania 2 
            public TemplateWorkerParametry(Context context) : base(context)
            {
                this.DataObliczen = Date.Today;
            }
        }
        //Obiekt Context jest to pudełko które przechowuje Typy danych, aktualnie załadowane w aplikacji
        //Atrybut Context pobiera z "Contextu" obiekty które aktualnie widzimy na ekranie
        [Context]
        public Context Cx { get; set; }
        //Pobieramy z Contextu parametry, jeżeli nie ma w Context Parametrów mechanizm sam utworzy nowy obiekt oraz wyświetli jego formatkę
        [Context]
        public TemplateWorkerParametry Parametry { get; set; }
        //Atrybut Action - Wywołuje nam metodę która znajduje się poniżej
        [Action("Kalkulator",
           Description = "Prosty kalkulator ",
           Priority = 10,
           Mode = ActionMode.ReadOnlySession,
           Icon = ActionIcon.Accept,
           Target = ActionTarget.ToolbarWithText)]
        public void WykonajAkcje()
        {

            int wynik = 0;

            // Zadanie 1 

            wynik = Oblicz(Parametry.ZmiennaA, Parametry.ZmiennaB, Parametry.Operacja);

            // Zadanie 2 

          //  wynik = ObliczPole(Parametry.ZmiennaA, Parametry.ZmiennaB, Parametry.figura);

            // Zadanie 3 


           // wynik  = ObliczPole(MyParser( Parametry.ZmiennaA), MyParser( Parametry.ZmiennaB), Parametry.figura);

            //Włączenie Debug, aby działał należy wygenerować DLL w trybie DEBUG
            DebuggerSession.MarkLineAsBreakPoint();
            //Pobieranie danych z Contextu
            Pracownik pracownik = null;
            if (this.Cx.Contains(typeof(Pracownik)))
            {
                pracownik = (Pracownik)this.Cx[typeof(Pracownik)];
            }

            //Modyfikacja danych
            //Aby modyfikować dane musimy mieć otwartą sesję, któa nie jest read only
            using (Session nowaSesja = this.Cx.Login.CreateSession(false, false, "ModyfikacjaPracownika"))
            {
                //Otwieramy Transaction aby można było edytować obiekt z sesji
                using (ITransaction trans = nowaSesja.Logout(true))
                {
                    //Pobieramy obiekt z Nowo utworzonej sesji
                    var pracownikZSesja = nowaSesja.Get(pracownik);
                    //Features - są to pola rozszerzające obiekty w bazie danych, dzięki czemu nie jestesmy ogarniczeni to kolumn jakie zostały utworzone przez producenta
                    pracownikZSesja.Features["DataObliczen"] = this.Parametry.DataObliczen;
                    pracownikZSesja.Features["Wynik"] = wynik;
                    //Zatwierdzamy zmiany wykonane w sesji
                    trans.CommitUI();
                }
                //Zapisujemy zmiany
                nowaSesja.Save();
            }
        }

        private int MyParser (string strvalue)
        {
            int retval = 0;
            int multiplier = 1;
            bool isMinus = false;
            List<char> chars = strvalue.ToList();
            if(chars.First() == '-')
            {
                isMinus = true;
                chars.RemoveAt(0);
            }
            chars.Reverse();
            foreach (var item in chars)
            {
                retval = retval + ReadNumberChar(item) * multiplier;

                multiplier = multiplier * 10;
            }

            return isMinus? retval * -1: retval;
        }

        private int ReadNumberChar (char ch)
        {
            int retval = 0;
            switch (ch)

            {
                case '1':
                    retval = 1;
                    break;
                case '2':
                    retval = 2;
                    break;
                case '3':
                    retval = 3;
                    break;
                case '4':
                   
                    retval = 4;
                    break;
                case '5':
                    retval = 5;
                    break;
                case '6':
                    retval = 6;
                    break;
                case '7':
                    retval = 7;
                    break;
                case '8':

                    retval = 8;
                    break;
                case '9':
                    retval = 9;
                    break;
                case '0':
                    retval = 0;
                    break;
              
                default:
                    throw new FormatException("Nieprawidłowy format ciągu wejściowego");
                    break;
            }
            return retval;
        }

        private int Oblicz (int a,int b ,string znak)
        {
            int retval = 0;
            switch (znak)
            {
                case "+":
                    retval = a+b;
                    break;
                case "-":
                    retval = a-b;
                    break;
                case "*":
                    retval = a*b;
                    break;
                case "/":
                    if (b == 0) throw new Exception("Nie można dzielic przez zero!");
                    retval = a / b;
                    break;
                default:
                    break;
            }
            return retval;
        }

        private int ObliczPole(int a, int b, Figura figura)
        {
            int retval = 0;
            switch (figura)
            {
                case Figura.kwadrat:
                    retval = a * a;
                    break;
                case Figura.prostokąt:
                    retval = a * b;
                    break;
                case Figura.trójkąt:
                    retval = a * b/2;
                    break;
                case Figura.koło:
                    
                    retval = (int)(a *a*Math.PI);
                    break;
                default:
                    break;
            }
            return retval;
        }
    }
    public enum Figura
    {
        kwadrat, prostokąt, trójkąt, koło
    }
}