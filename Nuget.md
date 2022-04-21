# Nexus-Repository

Damit Visual Studio die **Nuget-Packages** auch ohne Internet laden kann, muss eine Änderung in der Einstellung vorgenommen werden. Die nachfolgende Anleitung beschreibt die Änderung Schritt für Schritt.

## Änderung der Einstellung - Package Sources

Die Einstellung **Package Sources** gibt an, von welchem Server die Nuget-Packages geladen werden. Um diese Einstellung zu ändern, führen Sie nun die folgenden Schritte durch:  

* Visual Studio starten
* Aktivieren Sie den Menüpunkt **Tools -> Optionen...**
* Geben Sie im Suchfeld '**nuget**' ein
* Wählen Sie den Eintrag '**Package Sources**' aus  
* Fügen Sie einen Eintrag mit '**+**' hinzu und bennenen Sie diesen mit '**NexusRepo**'  
* Geben Sie im Feld Source: http://nrm.htl-leonding.ac.at:8081/repository/nuget-proxy/index.json
ein  
* Bestätigen Sie den Eintrag mit dem Button 'Update'  
* Aktivieren Sie die Auswahl (CheckBox)
 
![Screenshot](Screenshot.png)