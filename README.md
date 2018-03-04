<h1 align="center"> Jubilant Waffle </h1>
<h2 align="center"> Programmazione di sistema </h2>
<h6 align="center"> Anno accademico 2016-2017 </h6>


### Progetto del corso
#### Condivisione di file tramite rete LAN
Scopo del progetto è l’implementazione di una soluzione che consenta di inviare file o cartelle
tra due o più computer sfruttando la rete LAN come mezzo di trasmissione.
### Descrizione del sistema
Il servizio di sharing, messo in esecuzione in background, consente di annunciare la
disponibilità del proprio host a tutti gli altri host presenti nella rete locale. Per scoprire la
presenza di tali host, potrà essere utilizzato un meccanismo basato sull’uso di pacchetti UDP
multicast o tecnologie simili.

Un host può essere configurato in modalità privata: in tal caso non rivelerà agli altri host la
sua presenza e non potrà ricevere alcun file. Potrà però inviarli.
Un host configurato in modalità pubblica, invece, annuncia la propria presenza, e consente di
accettare file provenienti da altri computer della rete, comportandosi da server.
Se l’utente di un host seleziona il menu contestuale (tasto destro) su un file o su una cartella,
vede apparire una voce che gli consente di condividere il file o la cartella a tutti gli host che si
sono annunciati o solo a un loro sottoinsieme. In questo caso l’host si comporta da client.
La connessione tra i diversi host coinvolti nel trasferimento sarà basata su un protocollo di
rete a piacere.

Un host che riceve la richiesta di invio di un file può attendere esplicita conferma da parte
dell’utente, oppure può essere configurato per accettare automaticamente tutti i file. Può
chiedere all’utente il percorso in cui memorizzare i file ricevuti, oppure usare un percorso di
default. Ogni host mantiene l’identità dell’utente attualmente connesso e con tale identità si
annuncia in rete. Tali comportamenti possono essere configurati tramite le impostazioni
dell’applicazione.
Un host può accettare file da parte di più host contemporaneamente.

Bisognerà gestire i conflitti in caso di ricezione di più file con stesso nome da salvare sullo
stesso percorso.

L’host che invia un file visualizza una barra di avanzamento, una stima del tempo di
completamento, e dà all’utente la possibilità di annullare l’operazione.
