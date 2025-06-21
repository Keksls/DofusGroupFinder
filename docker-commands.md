# 📦 Zaapix – Docker Commands Reference

## 📍 Démarrer tout (API + DB + migrations auto)
```bash
docker compose up --build
```

---

## 🔄 Rebuilder un service précis

### Rebuild & relancer l'API seule
```bash
docker compose up --build api
```

### Rebuild et exécuter uniquement le migrator
```bash
docker compose run --rm migrator
```

### Rebuild et exécuter uniquement le baker
```bash
docker compose run --rm baker
```

---

## 🚀 Lancer les services

### Lancer l'API en détaché
```bash
docker compose up -d api
```

### Lancer tout en détaché
```bash
docker compose up -d
```

---

## 🧪 Voir les logs

### Logs en direct de l’API
```bash
docker compose logs -f api
```

### Logs de Postgres
```bash
docker compose logs -f postgres
```

---

## 🧼 Réinitialiser toute la base

### Supprimer les conteneurs, volumes et rebuild + migrations + seed
```bash
docker compose down -v
docker compose run --rm migrator
docker compose run --rm baker
docker compose up -d api
```

---

## 🔧 Maintenance

### Supprimer tous les conteneurs, images, caches et volumes inutiles
```bash
docker system prune -af --volumes
```

---

## 🧭 Infos complémentaires

### Voir les conteneurs actifs
```bash
docker ps
```

### Supprimer un conteneur existant par nom
```bash
docker rm -f zaapix-api
```

### Supprimer une image manuellement
```bash
docker rmi <image_id>
```

---
