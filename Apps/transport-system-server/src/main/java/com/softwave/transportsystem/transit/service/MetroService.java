package com.softwave.transportsystem.transit.service;

import com.softwave.transportsystem.transit.model.MetroLine;
import com.softwave.transportsystem.transit.repository.MetroLineRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class MetroService {

    private final MetroLineRepository metroLineRepository;

    public MetroService(MetroLineRepository metroLineRepository) {
        this.metroLineRepository = metroLineRepository;
    }

    public List<MetroLine> findAll() {
        return metroLineRepository.findAll();
    }

    public Optional<MetroLine> findById(String lineId) {
        return metroLineRepository.findById(lineId);
    }
}
