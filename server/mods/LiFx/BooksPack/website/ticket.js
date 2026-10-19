// Ticket-Abfrage fuer den Spiel-Client: der Client oeffnet die Schreibseite mit ?ticket=<zufaellig> und fragt hier ab, bis das
// Schriftstueck abgeschlossen ist. Antwort (Klartext): erste Zeile = Code (Tag), zweite Zeile = Titel. Solange nichts fertig ist: 404.
// Das Ticket ist ein vom Client erzeugter Zufallswert; es gibt nur Code und Titel preis (wie /api/title).
const express = require("express");
const db = require("../db");

const router = express.Router();

router.get("/:ticket", (req, res) => {
  const t = req.params.ticket;
  if (!/^[A-Za-z0-9]{8,24}$/.test(t)) return res.status(404).type("text/plain").send("");
  const doc = db
    .prepare("SELECT id, title FROM documents WHERE ticket = ? AND finalized = 1 ORDER BY created_at DESC LIMIT 1")
    .get(t);
  if (!doc) return res.status(404).type("text/plain").send("");
  res.type("text/plain").send(doc.id + "\n" + (doc.title || "").split("\n")[0].slice(0, 100));
});

module.exports = router;
