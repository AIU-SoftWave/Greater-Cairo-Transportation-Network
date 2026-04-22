package com.softwave.transportsystem.transit.repository;

import com.softwave.transportsystem.transit.model.MetroLine;
import org.springframework.data.jpa.repository.JpaRepository;

public interface MetroLineRepository extends JpaRepository<MetroLine, String> {
}
