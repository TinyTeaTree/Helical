using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public abstract class GameInfra : MonoBehaviour, IBootstrap
    {
        public static GameInfra Single;
        
        protected TypeSet<IFeature> _features = new();
        protected TypeSet<IService> _services = new();
        protected TypeSet<IAgent> _agents = new();
        protected Dictionary<Type, BaseFactory> _factories = new();
        protected Dictionary<Type, BaseRecord> _records = new();

        public TypeSet<IFeature> Features => _features;
        public TypeSet<IService> Services => _services;
        public TypeSet<IAgent> Agents => _agents;
        public Dictionary<Type, BaseFactory> Factories => _factories;
        public Dictionary<Type, BaseRecord> Records => _records;
        
        private void Awake()
        {
            if (Single == null)
            {
                Single = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            AddServices();
            AddAgents();
            AddFeatures();
            AddFactories();
            AddRecords();
            SetupAgents();
            BootstrapCustoms();
            BootstrapFeatures();
            StartGame();
        }

        protected abstract void AddServices();
        
        protected abstract void AddFeatures();

        protected abstract void AddFactories();

        protected abstract void AddAgents();

        protected abstract void AddRecords();
        
        private void BootstrapFeatures()
        {
            var all = _features.GetAll();
            foreach (var f in all)
            {
                f.Bootstrap(this);
            }
        }

        protected virtual void BootstrapCustoms()
        {
        }

        private void SetupAgents()
        {
            var allAgents = _agents.GetAll();
            foreach (var agent in allAgents)
            {
                (agent as BaseAgent).Setup(_features.GetAll(), _services.GetAll());
            }
        }

        protected abstract void StartGame();
    }
}