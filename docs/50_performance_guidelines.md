### Performance Guidelines for Adventure_God v1.1

This document outlines performance targets, optimization strategies, and monitoring procedures for the Unity + Node.js gateway architecture.

## Performance Targets

### Frame Rate Requirements
- **Target**: 60 FPS minimum for smooth gameplay
- **Acceptable**: 30 FPS minimum during heavy operations
- **Critical**: Never drop below 20 FPS for more than 1 second

### Memory Usage Limits
- **Unity Runtime**: < 500MB total memory usage
- **Save/Load Operations**: < 100MB temporary memory spike
- **WebSocket Messages**: < 1MB total message size
- **Scene Assets**: < 200MB loaded assets

### WebSocket Performance
- **Latency**: < 100ms round-trip time
- **Message Frequency**: < 10 messages/second
- **Connection Stability**: 99.9% uptime

## Unity Performance Optimization

### Rendering Optimization
- **Use URP (Universal Render Pipeline)** for optimal performance
- **Static Batching**: Enable for static scene objects
- **Dynamic Batching**: Use for small, frequently updated objects
- **LOD (Level of Detail)**: Implement for complex objects
- **Occlusion Culling**: Enable for large scenes

### Memory Management
- **Object Pooling**: Use for frequently created/destroyed objects
- **Asset Loading**: Implement async loading for large assets
- **Garbage Collection**: Minimize allocations in Update loops
- **Texture Compression**: Use appropriate compression for all textures
- **Mesh Optimization**: Reduce polygon count where possible

### Script Optimization
```csharp
// Good: Cache component references
private Transform _transform;
private void Awake() {
    _transform = transform;
}

// Bad: GetComponent in Update
private void Update() {
    transform.position = newPosition; // Allocates Vector3
}

// Good: Reuse Vector3
private Vector3 _position = Vector3.zero;
private void Update() {
    _position.Set(x, y, z);
    _transform.position = _position;
}
```

### UI Performance
- **Canvas Optimization**: Use separate canvases for static/dynamic UI
- **Text Optimization**: Use TextMeshPro for better performance
- **Image Optimization**: Use sprite atlases for UI images
- **Layout Optimization**: Minimize layout rebuilds

## Gateway Performance Optimization

### WebSocket Optimization
```typescript
// Good: Batch messages when possible
const batchMessages = (messages: any[]) => {
    return messages.map(msg => JSON.stringify(msg)).join('\n');
};

// Good: Implement message queuing
class MessageQueue {
    private queue: any[] = [];
    private processing = false;
    
    async process() {
        if (this.processing) return;
        this.processing = true;
        
        while (this.queue.length > 0) {
            const batch = this.queue.splice(0, 10); // Process 10 at a time
            await this.sendBatch(batch);
        }
        
        this.processing = false;
    }
}
```

### Memory Management
- **Connection Pooling**: Reuse WebSocket connections
- **Message Validation**: Validate before processing to avoid memory leaks
- **Error Handling**: Proper cleanup on errors
- **Logging**: Implement log rotation to prevent disk space issues

## Performance Monitoring

### Unity Profiler Setup
```csharp
// Performance monitoring script
public class PerformanceMonitor : MonoBehaviour
{
    [SerializeField] private float targetFPS = 60f;
    [SerializeField] private float memoryThreshold = 500f; // MB
    
    private void Update()
    {
        // Monitor FPS
        float currentFPS = 1f / Time.unscaledDeltaTime;
        if (currentFPS < targetFPS * 0.8f) // 80% of target
        {
            Debug.LogWarning($"Low FPS detected: {currentFPS:F1}");
        }
        
        // Monitor memory
        float memoryUsage = System.GC.GetTotalMemory(false) / 1024f / 1024f; // MB
        if (memoryUsage > memoryThreshold)
        {
            Debug.LogWarning($"High memory usage: {memoryUsage:F1}MB");
        }
    }
}
```

### Gateway Performance Monitoring
```typescript
// Performance monitoring for gateway
class PerformanceMonitor {
    private messageCount = 0;
    private startTime = Date.now();
    
    trackMessage() {
        this.messageCount++;
        const elapsed = Date.now() - this.startTime;
        const rate = this.messageCount / (elapsed / 1000);
        
        if (rate > 10) { // More than 10 messages/second
            console.warn(`High message rate: ${rate.toFixed(1)}/s`);
        }
    }
    
    trackLatency(startTime: number) {
        const latency = Date.now() - startTime;
        if (latency > 100) { // More than 100ms
            console.warn(`High latency: ${latency}ms`);
        }
    }
}
```

## Optimization Checklist

### Pre-Release Performance Check
- [ ] **Frame Rate**: Maintain 60 FPS during normal gameplay
- [ ] **Memory Usage**: Stay under 500MB during extended play
- [ ] **WebSocket Latency**: < 100ms round-trip time
- [ ] **Save/Load**: Complete within 2 seconds
- [ ] **UI Responsiveness**: All UI interactions respond within 100ms
- [ ] **Asset Loading**: Large assets load asynchronously
- [ ] **Garbage Collection**: No GC spikes during gameplay

### Performance Testing Scenarios
1. **Normal Gameplay**: 30 minutes of continuous play
2. **Heavy Operations**: Multiple save/load operations
3. **Stress Test**: Rapid UI interactions and movements
4. **Memory Test**: Extended play with multiple scene loads
5. **Network Test**: High-frequency WebSocket communication

## Performance Debugging

### Common Performance Issues

**Low Frame Rate**
- Check for expensive operations in Update loops
- Profile with Unity Profiler
- Look for garbage collection spikes
- Verify rendering settings

**High Memory Usage**
- Check for memory leaks in object pooling
- Monitor asset loading and unloading
- Verify texture compression settings
- Check for large data structures

**WebSocket Issues**
- Monitor message frequency and size
- Check for connection pooling
- Verify error handling and cleanup
- Monitor network latency

### Debugging Tools
- **Unity Profiler**: For Unity performance analysis
- **Chrome DevTools**: For WebSocket debugging
- **Node.js Profiler**: For gateway performance
- **Memory Profiler**: For memory leak detection

## Future Performance Enhancements

### Planned Optimizations
- **LOD System**: Implement for complex game objects
- **Asset Streaming**: Dynamic asset loading based on proximity
- **Multi-threading**: Offload heavy calculations to background threads
- **GPU Instancing**: For rendering multiple similar objects
- **Compression**: Implement message compression for WebSocket
- **Caching**: Add intelligent caching for frequently accessed data

### Performance Metrics Dashboard
- Real-time FPS monitoring
- Memory usage tracking
- WebSocket performance metrics
- Error rate monitoring
- User experience metrics
