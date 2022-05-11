AspMvc-WebApp  
=============  
  
Mit dem **AspMvc-Framework** koennen sehr einfach Web-Anwendungen erstellt werden. Web-Anwendungen haben den Vorteil, dass diese Anwendungen ortsungebunden und geraeteunabhaengig ausgefuehrt werden koennen. Einzig und allein ein Zugang zum Internet ist dafuer erforderlich. Diese Art von Anwendung, ist aus unserem Alltag nicht mehr wegzudenken.  
  
Der **AspMvc-Framework** ist ein Teil vom **DotNet-Core-Framework** und kann mit der Programmiersprache CSharp angepasst werden. Dies hat den Vorteil, dass diese Anwendung direkt mit dem Backend-System kommunizieren kann. Es ist also kein RESTful-Service fuer den Zugriff auf die Logik erforderlich.   
  
# Funktionsweise  
  
Wenn eine Url in den Browser eingegeben wird, dann wird diese vom Framework entsprechend ausgwertet und an die Komponenten der Anwendung weitergeleitet. Die Aufloesung der Url erfolgt in:  
  
- Server  
- Kontroller  
- Aktion (mit und ohne Parameter)  
  
Zum Beispiel wird die Url https://musicstorelight.com/genres/create wie folgt aufgeloest:  
  
- https://musicstorelight  
- genres  
- create  
  
Der Server-Teil ist die Adresse des Host-Servers und ist fuer den AspMvc-Framework ohne Bedeutung. Im zweiten Teil ist der Kontroller adressiert. Diese Information weist den Framework an, fuer die Anfrage den Kontroller ***GenresController*** zu verwenden. Dieser Kontroller befindet sich im Abschnitt ***Controllers*** und muss nach der Nameskonvention mit **Controller** enden. Der dritte und letzte Teil, die Aktion, zeigt den Framework an, welche Methode fuer die Anfrage zustaendig ist. Im Beispiel ist das die **Create()-Methode** des Kontrollers ***GenresController***.  
  
 ## Verbindung Aktion mit der Ansicht  
  
Mit der **Aktion** ist eine **Ansicht** assoziiert. Diese Assoziation erfolgt ueber die Rueckgabe durch ***'return View(model)'*** in der Aktionsmethode. Dabei muss folgende Namenskonvention beachtet werden.  
Der Framework sucht zuerst im Ordner **Views** nach einem Unterordner mit dem Namen des Kontrollers. Also nach einem Unterordner mit dem Namen ***Genres***. Gibt es einen solchen Unterordner, dann sucht der Framework nach einer Ansicht (View) mit dem Namen der Aktion. Gibt es in diesem Ordner eine Ansicht mit dem Namen ***Create.cshtml***, dann wird diese ausgewaehlt und an die Render-Engine zurueck geliefert.  
Findet der Framework keine Ansicht mit dem Namen ***Create.cshtml***, dann sucht der Framework im Ordner ***'Shared'*** weiter nch dieser Ansicht. Findet der Framework keine Ansicht mit diesem Namen, dann reagiert dieser mit einer Fehlermeldung.  
  
> HINWEIS: Es ist auch moeglich, einen anderen Namen fuer die Ansicht zu verwenden. Allerdings muss dieser in der Rueckgabe ***'return View("MyCreate", model)'*** angegeben werden.  
  
## Ablauf und Interaktion  
  
In der nachfolgenden Skizze ist der Ablauf schematisch dargestellt:  
   
  
![AspMvcDataFlow](AspMvcDataFlow.png)  
