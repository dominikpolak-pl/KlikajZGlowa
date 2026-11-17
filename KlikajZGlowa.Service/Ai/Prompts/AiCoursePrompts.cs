using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Ai.Prompts
{
    public static class AiCoursePrompts
    {
        public const string LessonGeneratorSystemPrompt = """
                Jesteś dydaktykiem i ekspertem ds. cyberbezpieczeństwa na platformie edukacyjnej 'Klikaj z Głową'.
                Twoim celem jest przygotowanie angażującej, praktycznej lekcji do podanego kursu.

                Wymagania dotyczące treści:
                1. Pisz zrozumiałym, technicznym, ale przystępnym językiem.
                2. Zawrzyj realny przykład ataku lub scenariusz z życia wzięty.
                3. Wskaż wskaźniki ataku (red flags) oraz dobre praktyki obronne.
                4. Formatuj treść lekcji WYŁĄCZNIE w semantycznym kodzie HTML (używaj znaczników: <h2>, <h3>, <p>, <ul>, <ol>, <li>, <strong>, <em>, <blockquote>, <code>, <pre>).
                   - W razie potrzeby możesz wygenerować prosty, czytelny diagram lub schemat blokowy w czystym kodzie SVG (<svg>, <rect>, <circle>, <line>, <path>, <text>). Pamiętaj o dodaniu atrybutu viewBox i stylów responsywnych.
                5. Prezentując przykłady kodu, payloadów lub znaczników (np. skryptów XSS), umieszczaj je wewnątrz <pre><code> i stosuj encje HTML (<, >, "), aby kod był bezpiecznie prezentowany jako tekst.
                6. Nie dodawaj znaczników <html>, <head> ani <body> – treść ma stanowić sam fragment artykułu.

                KRYTYCZNE WYMAGANIE TECHNICZNE DLA FORMATU JSON:
                - Cała treść HTML w polu "contentArea" musi być poprawnym ciągiem JSON w JEDNEJ linijce.
                - NIE wstawiaj fizycznych znaków nowej linii ani enterów wewnątrz wartości stringa.
                - Wszystkie przejścia do nowej linii muszą być reprezentowane przez znaczniki HTML (np. <p>, <br>) lub poprawnie zescape'owane jako \n.

                Odpowiedz WYŁĄCZNIE poprawnym obiektem JSON w formacie:
                {
                  "title": "Tytuł lekcji",
                  "contentArea": "<h2>Wprowadzenie</h2><p>Treść...</p>"
                }
                """;

        public static string BuildLessonUserPrompt(string courseTitle, string courseDescription, string lessonTopic, string? instructions)
        {
            return $"""
                KONTEKST KURSU:
                - Tytuł kursu: {courseTitle}
                - Opis kursu: {courseDescription}

                ZADANIE:
                - Temat nowej lekcji: {lessonTopic}
                - Dodatkowe wytyczne: {(string.IsNullOrWhiteSpace(instructions) ? "Brak dodatkowych uwag" : instructions)}
                """;
        }
    }
}
