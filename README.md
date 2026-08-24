# 🚀 Middlewares

[![Go Reference](https://pkg.go.dev/badge/github.com/sunmeat/middlewares.svg)](https://pkg.go.dev/github.com/sunmeat/middlewares)
[![Go Report Card](https://goreportcard.com/badge/github.com/sunmeat/middlewares)](https://goreportcard.com/report/github.com/sunmeat/middlewares)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Build Status](https://github.com/sunmeat/middlewares/workflows/CI/badge.svg)](https://github.com/sunmeat/middlewares/actions)

A lightweight, modular, and high-performance middleware collection for Go web applications and HTTP services.

---

## 📋 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Available Middlewares](#-available-middlewares)
  - [Logging & Metrics](#logging--metrics)
  - [Security](#security)
  - [Request Management](#request-management)
- [Usage Examples](#-usage-examples)
- [Configuration](#-configuration)
- [Benchmarks](#-benchmarks)
- [Contributing](#-contributing)
- [License](#-license)

---

## ✨ Features

- **Zero Allocation Mindset:** Optimized for high-throughput HTTP servers.
- **Idiomatic Go:** Seamlessly interfaces with `net/http` (`http.Handler` / `http.HandlerFunc`).
- **Composable:** Chain multiple middlewares easily without performance penalties.
- **Production Ready:** Built-in rate limiting, CORS, panic recovery, structured logging, and security headers.

---

## 📦 Installation

```bash
go get -u github.com/sunmeat/middlewares
```

---

## ⚡ Quick Start

```go
package main

import (
	"log"
	"net/http"
	"time"

	"github.com/sunmeat/middlewares"
)

func main() {
	mux := http.NewServeMux()

	// Sample Handler
	mux.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		w.Write([]byte("Hello, World!"))
	})

	// Wrap handlers with middleware chain
	handler := middlewares.Chain(
		mux,
		middlewares.Recovery(),
		middlewares.Logger(),
		middlewares.CORS(middlewares.DefaultCORSConfig),
	)

	server := &http.Server{
		Addr:         ":8080",
		Handler:      handler,
		ReadTimeout:  5 * time.Second,
		WriteTimeout: 10 * time.Second,
	}

	log.Println("Server running on http://localhost:8080")
	if err := server.ListenAndServe(); err != nil {
		log.Fatalf("Server failed: %v", err)
	}
}
```

---

## 🛠 Available Middlewares

| Middleware | Description | Configurable |
| :--- | :--- | :---: |
| **`Logger`** | Structured HTTP request & response logging with response times | ✅ |
| **`Recovery`** | Panics catcher with stack trace reporting to prevent crashes | ✅ |
| **`CORS`** | Cross-Origin Resource Sharing handling with custom domains | ✅ |
| **`RateLimiter`** | Token bucket / Leaky bucket request throttler | ✅ |
| **`SecurityHeaders`** | Sets OWASP recommended security headers (HSTS, CSP, X-Frame) | ✅ |
| **`RequestID`** | Generates or propagates unique `X-Request-ID` headers | ✅ |

---

## 💡 Usage Examples

### Custom Logger & Request ID

```go
loggerConfig := middlewares.LoggerConfig{
	IncludeHeaders: true,
	SkipPaths:      []string{"/healthz", "/metrics"},
}

handler := middlewares.Chain(
	mux,
	middlewares.RequestID(),
	middlewares.LoggerWithConfig(loggerConfig),
)
```

### Rate Limiting

```go
limiter := middlewares.RateLimiter(middlewares.RateLimiterConfig{
	RequestsPerSecond: 100,
	Burst:             200,
})

http.Handle("/api/", limiter(apiMux))
```

---

## 🧪 Testing & Benchmarks

Run unit tests and benchmarks locally:

```bash
# Run tests with race detection
go test -v -race ./...

# Run benchmarks
go test -bench=. -benchmem ./...
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create your feature branch (`git checkout -b feature/AmazingFeature`).
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4. Push to the branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

---

## 📄 License

Distributed under the MIT License. See [`LICENSE`](LICENSE) for more information.
