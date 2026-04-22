package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;
import org.springframework.data.jpa.repository.JpaRepository;

public interface NodeRepository extends JpaRepository<AbstractNode, String> {
}
